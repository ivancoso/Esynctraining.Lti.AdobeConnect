package examples.weborb;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.widget.EditText;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;

public class ConnectionPreferencesActivity extends Activity
{
  private static final int TEST_SUCCESS = 0;
  private static final int TEST_FAILED = 1;

  public static final int PROPERTIES_UPDATED = 0;
  public static final int PROPERTIES_ARE_SAME = 1;

  private EditText mWeborbURLEditText;
  private String[] dialog_titles;
  private String[] dialog_messages;
  private String weborbUrl;

  @Override
  protected void onCreate( Bundle savedInstanceState )
  {
    super.onCreate( savedInstanceState );
    setTitle( R.string.weborb_configurations );
    setContentView( R.layout.preferences_layout );
    dialog_titles = getResources().getStringArray(R.array.connection_dialog_titles);
    dialog_messages = getResources().getStringArray(R.array.connection_dialog_messages);
    findViewById( R.id.accept_button ).setOnClickListener( new View.OnClickListener()
    {
      @Override
      public void onClick( View view )
      {
        SharedPreferences.Editor editor = getSharedPreferences( getString( R.string.weborb_configurations ),
                                                              MODE_WORLD_READABLE ).edit();
        weborbUrl = mWeborbURLEditText.getText().toString();
        editor.putString( getString( R.string.weborb_endpoint_url ), weborbUrl );
        editor.apply();
        setResult( PROPERTIES_UPDATED );
        finish();
      }
    } );

    findViewById( R.id.test_connection_button ).setOnClickListener( new View.OnClickListener()
    {
      @Override
      public void onClick( View view )
      {
        weborbUrl = mWeborbURLEditText.getText().toString();
        new TestConnectionThread(weborbUrl).start();
      }
    } );

    mWeborbURLEditText = (EditText) findViewById( R.id.url );
    SharedPreferences settings = getSharedPreferences( getString(R.string.weborb_configurations), MODE_WORLD_READABLE );
    String weborbTag = getString(R.string.weborb_endpoint_url);
    weborbUrl = settings.getString( weborbTag, getIntent().getStringExtra( weborbTag ) );
    mWeborbURLEditText.setText( weborbUrl );
  }

  private Handler mTestConnectionHandler = new Handler(  )
  {
    @Override
    public void handleMessage( Message msg )
    {
      setProgressBarIndeterminateVisibility( false );

      int testResult = msg.what;

      AlertDialog.Builder builder = new AlertDialog.Builder( ConnectionPreferencesActivity.this );
      builder.setTitle( dialog_titles[ testResult ] );
      switch( testResult )
        {
        case TEST_SUCCESS:
          builder.setIcon( android.R.drawable.ic_dialog_info );
          break;
        case TEST_FAILED:
          builder.setIcon( android.R.drawable.ic_dialog_alert );
          break;
        }
      builder.setMessage( dialog_messages[ testResult ] );
      builder.setPositiveButton( "Ok", new DialogInterface.OnClickListener()
      {
        public void onClick( DialogInterface dialogInterface, int i )
        {
          dialogInterface.dismiss();
        }
      } );
      builder.show();
    }
  };

  private class TestConnectionThread extends Thread
  {
    private String testUrl;

    public TestConnectionThread(String testUrl)
    {
      super();

      this.testUrl = testUrl;
    }

    @Override
    public void run()
    {
      int testResult = testConnection( this.testUrl );
      mTestConnectionHandler.obtainMessage( testResult ).sendToTarget();
    }

    private int testConnection( String testUrl )
    {
      try
        {
        URL url = new URL( testUrl );
        HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
        urlConnection.setRequestMethod( "GET" );
        urlConnection.setConnectTimeout( 1000 );
        urlConnection.connect();
        int responseCode = urlConnection.getResponseCode();
        // check if response code in range of success http response codes
        if( responseCode >= 200 && responseCode <= 226 )
          return TEST_SUCCESS;
        }
      catch( MalformedURLException ignored )
        {
        }
      catch( ProtocolException ignored )
        {
        }
      catch( IOException ignored )
        {
        }
      return TEST_FAILED;
    }
  }
}
