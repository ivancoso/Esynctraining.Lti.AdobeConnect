package ui.broadcasting.viewer
{
	import flash.events.AsyncErrorEvent;
	import flash.events.NetStatusEvent;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	
	import ui.broadcasting.BroadcastingDefaults;
	import ui.managers.ConnectionManager;
	
	/**
	 * BroadcastViewerController gets commands from BroadcastViewer,
	 * processes data and changes (if it's necessary) BroadcastViewerModel data.
	 * <p>
	 * Broadcast viewer is an example how to subscribe to broadcast
	 * </p>
	 */
	public class BroadcastViewerController
	{
		/**
		 * Viewed video width
		 */
		public static const VIDEO_WIDTH:int = 160;
		
		/**
		 * Viewed video height
		 */
		public static const VIDEO_HEIGHT:int = 120;
		
		/**
		 * Constructor
		 */
		public function BroadcastViewerController(view:BroadcastViewer)
		{
			_view = view;
			
			// create connection and connect
			_netConnection = new NetConnection();
			_netConnection.client = this;
			_netConnection.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			_netConnection.connect(ConnectionManager.instance.connectionUri);
		}
		
		/**
		 * @private
		 */
		private var _view:BroadcastViewer;
		
		/**
		 * @private
		 * A two-way connection between a client and a server
		 */
		private var _netConnection:NetConnection;
		
		/**
		 * @private
		 * A one-way streaming channel over a NetConnection.
		 * We'll receive broadcasting media through this channel
		 */
		private var _netStream:NetStream;
		
		/**
		 * @private
		 * Instance to display broadcasting video
		 */
		private var _video:Video;
		
		/**
		 * Stop receiving media and closes the window
		 */
		public function onClose():void
		{
			stopViewing();
			_view.close();
		}
		
		/**
		 * @private
		 * Starts receiving broadcasting media
		 */
		private function startViewing():void
		{
			// create stream
			_netStream = new NetStream(_netConnection);
			_netStream.client = this;
			_netStream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			_netStream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			
			// create video container and attach stream to it
			_video = new Video(VIDEO_WIDTH, VIDEO_HEIGHT);
			_video.attachNetStream(_netStream);
			_view.videoContainer.addChild(_video);
			
			// start playing broadcasting media
			_netStream.play(BroadcastingDefaults.DEFAULT_STREAM_NAME);
		}
		
		/**
		 * @private
		 * Stops receiving broadcasting media
		 * Closes stream and netConnection
		 */
		private function stopViewing():void
		{
			if (_video)
			{
				_view.videoContainer.removeChild(_video);
				_video = null;
			}
			
			if (_netStream)
			{
				_netStream.removeEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
				_netStream.removeEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
				_netStream.close();
				_netStream = null;
			}
			
			if (_netConnection)
			{
				_netConnection.removeEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
				_netConnection.close();
				_netConnection = null;
			}
		}
		
		/**
		 * @private
		 * Handles connection and stream status events
		 */
		private function onNetStatus(event:NetStatusEvent):void
		{
			switch (event.info.code)
			{
				case "NetConnection.Connect.Success":
					startViewing();
					break;
				case "NetConnection.Connect.Failed":
					_view.lblError.text = "The connection attempt failed.";
					break;
				case "NetStream.Play.StreamNotFound":
					_view.lblError.text = "Stream is not found.";
					break;
				case "NetStream.Play.Failed":
					_view.lblError.text = "Playing is failed.";
					break;
			}
		}
		
		/**
		 * @private
		 */
		private function onAsyncError(event:AsyncErrorEvent):void
		{
			// ignore
		}
	}
}