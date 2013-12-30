package ui.enums
{
	/**
	 * Testing connection result types 
	 */
	public class ConnectionTestResultType
	{
		/**
		 * When application cannot connect to the server
		 */
		public static const BAD:int = 0; 
		
		/**
		 * When application can connect to the server
		 */
		public static const GOOD:int = 1;
		
		/**
		 * Undefinite state. We wait connecting
		 * result during 5 second.
		 */
		public static const TIMEOUT:int = 2;
	}
}