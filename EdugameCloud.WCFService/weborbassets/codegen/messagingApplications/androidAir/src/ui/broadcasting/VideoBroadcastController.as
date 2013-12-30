package ui.broadcasting
{
	import flash.events.AsyncErrorEvent;
	import flash.desktop.NativeApplication;
	import flash.events.NetStatusEvent;
	import flash.media.Camera;
	import flash.media.Microphone;
	import flash.media.Video;
	import flash.net.NetConnection;
	import flash.net.NetStream;
	import flash.net.URLRequest;
	import flash.net.navigateToURL;
	import flash.system.ApplicationDomain;
	import flash.system.LoaderContext;
	import flash.system.SecurityDomain;
	
	import mx.controls.SWFLoader;
	
	import ui.Defaults;
	import ui.broadcasting.viewer.BroadcastViewer;
	import ui.controls.Alert;
	import ui.managers.ConnectionManager;
	
	/**
	 * VideoBroadcastController gets commands from VideoBroadcastView,
	 * processes data and changes (if it's necessary) ViewBroadcastModel data.
	 */
	public class VideoBroadcastController
	{
		/**
		 * Constructor
		 */
		public function VideoBroadcastController(view:VideoBroadcastView)
		{
			_view = view;
			_model = new VideoBroadcastModel();
			
			_netConnection = new NetConnection();
			_netConnection.client = this;
			_netConnection.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
		}
		
		/**
		 * @private
		 */
		private var _view:VideoBroadcastView;
		
		/**
		 * @private
		 * A two-way connection between a client and a server
		 */
		private var _netConnection:NetConnection;
		
		/**
		 * @private
		 * A one-way streaming channel over a NetConnection.
		 * We'll broadcast media through this channel
		 */
		private var _netStream:NetStream;
		
		/**
		 * @private
		 */
		private var _camera:Camera;
		
		/**
		 * @private
		 */
		private var _mic:Microphone;
		
		/**
		 * @private
		 */
		private var _video:Video;
		
		/**
		 * @private
		 */
		private var _model:VideoBroadcastModel;
		
		/**
		 * Gets model to provide access to data.
		 */
		public function get model():VideoBroadcastModel
		{
			return _model;
		}
		
		/**
		 * Connects to the server if it's not connected yet.
		 * Otherwise disconnects.
		 */
		public function onConnect():void
		{
			if (!_model.connected)
				connect();
			else
				disconnect();
		}
		
		/**
		 * If it isn't broadcasting at the moment
		 * starts broadcast, otherwise stops it 
		 */
		public function onBroadcast():void
		{
			if (!_model.broadcasting)
				startBroadcast();
			else
				stopBroadcast();
		}
		
		/**
		 * Opens a new browser window/tab to view broadcast.
		 */
		public function onViewBroadcast():void
		{
			new BroadcastViewer().open(_view, true);
		}
		
		/**
		 * Closes connection and releases mic and camera
		 */
		public function onHome():void
		{
			disconnect();
		}
		
		/**
		 * @private
		 * Establishes connection between client and server
		 */
		private function connect():void
		{
			_netConnection.connect(ConnectionManager.instance.connectionUri);
		}
		
		/**
		 * @private
		 * Closes connection
		 */
		private function disconnect():void
		{
			// if it's broadcasting stop it first
			stopBroadcast();
			_netConnection.close();
		}
		
		/**
		 * @private
		 * Starts broadcast
		 */
		private function startBroadcast():void
		{
			// create stream
			_netStream = new NetStream(_netConnection);
			_netStream.client = this;
			_netStream.addEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
			_netStream.addEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
			
			// attach camera and mic
			_camera = Camera.getCamera();
			_mic = Microphone.getMicrophone();
			
			if (_camera)
			{
				_camera.setQuality(Defaults.BANDWIDTH, Defaults.QUALITY);
				_netStream.attachCamera(_camera);
				
				_video = new Video(Defaults.VIDEO_WIDTH, Defaults.VIDEO_HEIGHT);
				_video.attachCamera(_camera);
				_view.videoContainer.addChild(_video);
			}
			
			if (_mic)
				_netStream.attachAudio(_mic);
			
			// publish media to the server
			// pay attention! we publish with 'live' parameter
			// it means live broadcast
			// (compare with 'record' parameter that means media file
			// will be recorded to the server)
			_netStream.publish(BroadcastingDefaults.DEFAULT_STREAM_NAME, "live");
		}
		
		/**
		 * @private
		 * Stops broadcast
		 */
		private function stopBroadcast():void
		{
			if (_video)
			{
				_video.attachCamera(null);
				_view.videoContainer.removeChild(_video);
				_video = null;
			}
			
			if (_netStream)
			{
				// release resources
				_netStream.attachCamera(null);
				_netStream.attachAudio(null);
				
				// just close publishing stream
				_netStream.close();
			}
			
			// stop camera and mic
			_camera = null;
			_mic = null;
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
					_model.connected = true;
					break;
				case "NetConnection.Connect.Closed":
					_model.connected = false;
					_model.broadcasting = false;
					break;
				case "NetConnection.Connect.Failed":
					new Alert().show("The connection attempt failed.", _view, true);
					break;
				case "NetStream.Publish.BadName":
					new Alert().show("Attempt to publish a stream which is already being published by someone else.", _view, true);
					break;
				case "NetStream.Publish.Start":
					_model.broadcasting = true;
					break;
				case "NetStream.Connect.Failed":
					new Alert().show("The P2P connection attempt failed.", _view, true);
					break;
				case "NetConnection.Connect.Closed":
					_model.connected = false;
					// don't break here
				case "NetStream.Unpublish.Success":
					if (_netStream)
					{
						_netStream.removeEventListener(NetStatusEvent.NET_STATUS, onNetStatus);
						_netStream.removeEventListener(AsyncErrorEvent.ASYNC_ERROR, onAsyncError);
					}
					_model.broadcasting = false;
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