package ui.events
{
	import flash.events.Event;
	
	public class ConnectionTestEvent extends Event
	{
		/**
		 * Event type
		 */
		public static const TEST_RESULT:String = "testResult";
		
		/**
		 * Constructor
		 */
		public function ConnectionTestEvent(
			type:String, result:int, message:String = "", bubbles:Boolean = false, cancelable:Boolean = false)
		{
			super(type, bubbles, cancelable);
			_result = result;
			_message = message;
		}
		
		/**
		 * @private
		 */
		private var _result:int;
		
		/**
		 * Result of testing connection URI. 
		 * @see ui.enums.ConnectionTestResultType
		 */
		public function get result():int
		{
			return _result;
		}
		
		/**
		 * @private
		 */
		private var _message:String;
		
		/**
		 * Usually it's error message
		 */
		public function get message():String
		{
			return _message;
		}
		
		/**
		 * @private
		 */
		override public function clone():Event
		{
			return new ConnectionTestEvent(type, result, message, bubbles, cancelable);
		}
	}
}