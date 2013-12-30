package ui.home
{
	import ui.controls.Alert;
	import ui.enums.ConnectionTestResultType;
	import ui.events.ConnectionTestEvent;
	import ui.managers.ConnectionManager;
	
	/**
	 * HomeController gets commands from HomeView.
	 */
	public class HomeController
	{
		/**
		 * Constuctor
		 */
		public function HomeController(view:HomeView)
		{
			_view = view;
		}
		
		/**
		 * @private
		 */
		private var _view:HomeView;
		
		/**
		 * Sets appropriate icon and remembers the user defined URI
		 */
		public function onUriChange():void
		{
			if (_view.imgTestUri.source != _view.dpiBtmpSrcTest)
				_view.imgTestUri.source = _view.dpiBtmpSrcTest;
			
			ConnectionManager.instance.connectionUri = _view.txtConnectionUrl.text;
		}
		
		/**
		 * Tests the user defined URI
		 */
		public function onTestUri():void
		{
			if (_view.txtConnectionUrl.text == "")
			{
				new Alert().show("Input URI to test", _view, true);
				return;
			}
			
			ConnectionManager.instance.addEventListener(ConnectionTestEvent.TEST_RESULT, onTestResult);
			ConnectionManager.instance.testConnectionUri(_view.txtConnectionUrl.text);
			
			_view.busyIndicator.visible = true;
			_view.grContent.enabled = false;
		}
		
		/**
		 * @private
		 * Handles URI testing results
		 */
		private function onTestResult(event:ConnectionTestEvent):void
		{
			_view.busyIndicator.visible = false;
			_view.grContent.enabled = true;
			
			if (event.result == ConnectionTestResultType.GOOD)
			{
				_view.imgTestUri.source = _view.dpiBtmpSrcGood;
			}
			else
			{
				if (event.result == ConnectionTestResultType.BAD)
					_view.imgTestUri.source = _view.dpiBtmpSrcBad;
				
				new Alert().show(event.message, _view, true);
			}
			
			ConnectionManager.instance.removeEventListener(ConnectionTestEvent.TEST_RESULT, onTestResult);
		}
	}
}