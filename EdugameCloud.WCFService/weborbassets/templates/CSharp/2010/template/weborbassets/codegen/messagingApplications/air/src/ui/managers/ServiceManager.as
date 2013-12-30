package ui.managers
{
	import flash.net.NetConnection;
	
	import mx.messaging.config.ServerConfig;
	
	/**
	 * Helper class 
	 */
	public class ServiceManager
	{
		/**
		 * Gets connection string to the server
		 */
		public static function getUri():String
		{
			// TODO Если Tunneling false, то берем этот uri.
			
			var uri:String = ServerConfig.getChannel("air-nohttp").endpoint;
			// TODO Название приложения опреляется пользователем при выборе
			//      ветки дерева на вкладке Messaging Server в консоли weborb
			//      и будет передаваться на сервер в качетве параметра
			uri += "/MessagingCodegen";
			
			
			// TODO Если Tunneling true, то берем этот uri
			//      Аналогично, имя приложения опеределяется пользователем
			//      и будет передаваться на сервер в качетве параметра
//			var uri:String = "rtmpt://localhost" + "/MessagingCodegen";
			
			return uri;
		}
	}
}