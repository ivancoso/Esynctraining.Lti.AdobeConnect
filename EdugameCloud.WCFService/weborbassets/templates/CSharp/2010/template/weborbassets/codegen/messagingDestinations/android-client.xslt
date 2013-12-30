<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <folder name="weborb-codegen">
            <xsl:variable name="common_files_path" select="'messagingDestinations/android/common/'"/>
            <xsl:choose>
                <xsl:when test="data/fullCode = 'true'">
                    <folder name="Intellij Idea 10">
                        <folder name="MessageClientDemoApp">
                            <xsl:variable name="intellij_idea_files_path"
                                          select="'messagingDestinations/android/idea/MessageClientDemoApp/'"/>

                            <xsl:call-template name="codegen.project.intellij_idea">
                                <xsl:with-param name="app_files_path" select="$intellij_idea_files_path"/>
                            </xsl:call-template>
                            <xsl:call-template name="codegen.project.code">
                                <xsl:with-param name="common_files_path" select="$common_files_path"/>
                            </xsl:call-template>
                        </folder>
                    </folder>

                    <folder name="Eclipse 3.6">
                        <folder name="MessageClientDemoApp">
                            <xsl:variable name="eclipse_files_path" select="'messagingDestinations/android/eclipse/MessageClientDemoApp/'"/>

                            <xsl:call-template name="codegen.project.eclipse">
                                <xsl:with-param name="app_files_path" select="$eclipse_files_path"/>
                            </xsl:call-template>
                            <xsl:call-template name="codegen.project.code">
                                <xsl:with-param name="common_files_path" select="$common_files_path"/>
                            </xsl:call-template>
                        </folder>
                    </folder>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:call-template name="codegen.project.code">
                        <xsl:with-param name="common_files_path" select="$common_files_path"/>
                    </xsl:call-template>
                </xsl:otherwise>
            </xsl:choose>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.project.intellij_idea">
        <xsl:param name="app_files_path"/>

        <folder name="assets"/>
        <folder name="bin"/>
        <folder name="gen"/>
        <folder name="libs">
          <file path="../javaclient/weborbclient.jar"/>
        </folder>

        <file path="{concat($app_files_path, 'build.xml')}"/>
        <file path="{concat($app_files_path, 'build.properties')}"/>
        <file path="{concat($app_files_path, 'default.properties')}"/>
        <file path="{concat($app_files_path, 'MessageClientDemoApp.iml')}"/>
        <file path="{concat($app_files_path, 'local.properties')}"/>
        <file path="{concat($app_files_path, 'proguard.cfg')}"/>
        <folder name=".idea">
            <folder name="copyright">
                <file path="{concat($app_files_path, '.idea/copyright/profiles_settings.xml')}"/>
            </folder>
            <folder name="libraries">
                <file path="{concat($app_files_path, '.idea/libraries/weborbclient_jar.xml')}" hideContent="true"/>
            </folder>
            <file path="{concat($app_files_path, '.idea/.name')}"/>
            <file path="{concat($app_files_path, '.idea/ant.xml')}"/>
            <file path="{concat($app_files_path, '.idea/compiler.xml')}"/>
            <file path="{concat($app_files_path, '.idea/encodings.xml')}"/>
            <file path="{concat($app_files_path, '.idea/misc.xml')}"/>
            <file path="{concat($app_files_path, '.idea/modules.xml')}"/>
            <file path="{concat($app_files_path, '.idea/uiDesigner.xml')}"/>
            <file path="{concat($app_files_path, '.idea/vcs.xml')}"/>
            <file path="{concat($app_files_path, '.idea/workspace.xml')}"/>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.project.eclipse">
        <xsl:param name="app_files_path"/>

        <folder name="assets"/>
        <folder name="bin"/>
        <folder name="gen"/>
        <folder name="libs">
            <file path="../../javaclient/weborbclient.jar"/>
        </folder>
        <file path="{concat($app_files_path, '.classpath')}"/>
        <file path="{concat($app_files_path, '.project')}"/>
        <file path="{concat($app_files_path, 'default.properties')}"/>
        <file path="{concat($app_files_path, 'proguard.cfg')}"/>
    </xsl:template>

    <xsl:template name="codegen.project.code">
        <xsl:param name="common_files_path"/>

        <file path="{concat($common_files_path, 'AndroidManifest.xml')}"/>
        <folder name="src">
            <folder name="examples">
                <folder name="weborb">
                    <file path="{concat($common_files_path, 'src/examples/weborb/ConnectionPreferencesActivity.java')}"/>
                    <file name="MainActivity.java">
                        <xsl:text>package examples.weborb;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.inputmethod.EditorInfo;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import weborb.client.Fault;
import weborb.client.IResponder;
import weborb.client.Subscription;
import weborb.client.WeborbClient;
import weborb.exceptions.MessageException;
import weborb.types.IAdaptingType;
import weborb.v3types.AsyncMessage;

import java.io.IOException;

public class MainActivity extends Activity
{
  public static final int MESSAGE_RECEIVED = 0;
  public static final int ERROR_RECEIVED = 1;
  public static final int CONNECT_SUCCESS = 0;
  public static final int CONNECT_FAILED = 1;
  private static final int RECONNECT = Menu.FIRST;
  private static final int PROPERTIES = RECONNECT + 1;
  private static final String TAG = "WebORB Chat";
  private static final String CLIENT_ID_KEY = "WebORBClientId";
  private ArrayAdapter&lt;MessageRow&gt; mConversationArrayAdapter;
  private ListView mConversationView;
  private EditText mOutEditText;
  private Button mSendButton;
  private EditText mClientIdEditText;
  private String mClientId = null;
  private ConnectionThread mConnectionThread;
  private ProgressDialog mConnectionProgressDialog;
  private WeborbClient weborbClient;
  private Subscription subscription;
  private boolean mIsMessageSending = false;
  private String weborbUrl;
  private static final int CONNECTION_CONFIGURATION = 0;

  @Override
  public void onCreate( Bundle savedInstanceState )
  {
    super.onCreate( savedInstanceState );
    requestWindowFeature( Window.FEATURE_INDETERMINATE_PROGRESS );
    setContentView( R.layout.main );
    setProgressBarIndeterminateVisibility( false );
    readPreferences();
  }

  @Override
  public void onStart()
  {
    super.onStart();

    if( weborbClient == null )
      {
      setupChat();
      }
  }

  private void readPreferences()
  {
    SharedPreferences settings = getSharedPreferences( getString( R.string.weborb_configurations ),
                                                       MODE_WORLD_READABLE );
    weborbUrl = settings.getString( getString( R.string.weborb_endpoint_url ), "</xsl:text>
                <xsl:variable name="url" select="data/weborbURL"/>
                <xsl:choose>
                    <xsl:when test="contains($url, 'localhost')">
                        <xsl:value-of select="substring-before($url, 'localhost')"/>
                        <xsl:text>10.0.2.2</xsl:text>
                        <xsl:value-of select="substring-after($url, 'localhost')"/>
                    </xsl:when>
                    <xsl:when test="contains($url, '127.0.0.1')">
                        <xsl:value-of select="substring-before($url, '127.0.0.1')"/>
                        <xsl:text>10.0.2.2</xsl:text>
                        <xsl:value-of select="substring-after($url, '127.0.0.1')"/>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of select="$url" /></xsl:otherwise>
                </xsl:choose>
                <xsl:text>" );
  }

  private void setupChat()
  {
    Log.d( TAG, "setupChat()" );

    mClientIdEditText = (EditText) findViewById( R.id.client_id );
    mClientIdEditText.setOnFocusChangeListener( new View.OnFocusChangeListener()
    {
      @Override
      public void onFocusChange( View view, boolean focused )
      {
        String clientId = mClientIdEditText.getText().toString().trim();
        if( !focused &amp;&amp; !clientId.equals( mClientId ) )
        {
          mClientId = clientId;
          reconnect();
        }
      }
    } );

    // Initialize the array adapter for the conversation thread
    mConversationArrayAdapter = new MessageArrayAdapter( this );
    mConversationView = (ListView) findViewById( R.id.in );
    mConversationView.setAdapter( mConversationArrayAdapter );

    // Initialize the compose field with a listener for the return key
    mOutEditText = (EditText) findViewById( R.id.edit_text_out );
    mOutEditText.setOnEditorActionListener( mWriteListener );

    // Initialize the send button with a listener that for click events
    mSendButton = (Button) findViewById( R.id.button_send );
    mSendButton.setOnClickListener( new View.OnClickListener()
    {
      public void onClick( View v )
      {
        // Send a message using content of the edit text widget
        TextView view = (TextView) findViewById( R.id.edit_text_out );
        String message = view.getText().toString();
        sendMessage( message );
      }
    } );

    mConnectionProgressDialog = new ProgressDialog( this );
    mConnectionProgressDialog.setMessage( getString( R.string.connection_message ) );
    mConnectionProgressDialog.setOnCancelListener( new DialogInterface.OnCancelListener()
    {
      @Override
      public void onCancel( DialogInterface dialogInterface )
      {
        setProgressBarIndeterminateVisibility( true );
      }
    } );
    mConnectionProgressDialog.show();
    mConnectionThread = new ConnectionThread();
    mConnectionThread.start();
  }

  private class ConnectionThread extends Thread
  {
    @Override
    public void run()
    {
      Log.i( TAG, "Making new connection" );
      if( subscription != null )
        {
        try
          {
          subscription.unsubscribe();
          }
        catch( IOException e )
          {
          Log.i( TAG, "Unable to unsubscribe", e );
          }
        finally
          {
          subscription = null;
          }
        }</xsl:text><xsl:variable name="destinationId" select="data/destinationId"/><xsl:text>
      if( mClientId == null || "".equals( mClientId ) )
        weborbClient = new WeborbClient( weborbUrl, "</xsl:text><xsl:value-of select="$destinationId"/><xsl:text>" );
      else
        weborbClient = new WeborbClient( weborbUrl, "</xsl:text><xsl:value-of select="$destinationId"/><xsl:text>", mClientId );
      makeConnection();
    }

    private void makeConnection()
    {
      try
        {
        subscription = weborbClient.subscribe( new IResponder()
        {
          public void responseHandler( Object adaptedObject )
          {
            Object[] resultArray = (Object[]) adaptedObject;

            for( Object message : resultArray )
              {
              AsyncMessage asyncMessage = (AsyncMessage) message;
              MessageRow messageRow = new MessageRow();
              String sender = (String) asyncMessage.headers.get( CLIENT_ID_KEY );
              if(sender == null)
                sender = "Anonymous";
              messageRow.title = sender;
              Object[] messages = (Object[]) asyncMessage.body.body;

              for( Object o : messages )
                {
                IAdaptingType adaptingObj = (IAdaptingType) o;
                messageRow.message = (String) adaptingObj.defaultAdapt();
                mHandler.obtainMessage( MainActivity.MESSAGE_RECEIVED, -1, -1, messageRow ).sendToTarget();
                }
              }
          }

          public void errorHandler( Fault fault )
          {
            MessageRow messageRow = new MessageRow();
            messageRow.title = "Error received";
            messageRow.message = fault.getDetail();
            mHandler.obtainMessage( MainActivity.ERROR_RECEIVED, -1, -1, messageRow ).sendToTarget();
          }
        } );
        Log.i( TAG, "Connected successfully" );
        mConnectionHandler.obtainMessage( CONNECT_SUCCESS ).sendToTarget();
        }
      catch( Exception e )
        {
        Log.i( TAG, "Failed connection" );
        mConnectionHandler.obtainMessage( CONNECT_FAILED, e.getMessage() ).sendToTarget();
        }
    }
  }

  private final Handler mConnectionHandler = new Handler()
  {
    @Override
    public void handleMessage( Message msg )
    {
      setProgressBarIndeterminateVisibility( false );

      if( mConnectionProgressDialog.isShowing() )
        mConnectionProgressDialog.dismiss();

      switch( msg.what )
        {
        case CONNECT_SUCCESS:
          break;
        case CONNECT_FAILED:
          AlertDialog.Builder errorDialogBuilder = new AlertDialog.Builder( MainActivity.this );
          errorDialogBuilder.setMessage( String.valueOf( msg.obj ) );
          errorDialogBuilder.setIcon( android.R.drawable.ic_dialog_alert );
          errorDialogBuilder.setTitle( R.string.connection_error_title );
          errorDialogBuilder.setPositiveButton( R.string.reconnect, new DialogInterface.OnClickListener()
          {
            public void onClick( DialogInterface dialogInterface, int i )
            {
              dialogInterface.dismiss();
              reconnect();
            }
          } );
          errorDialogBuilder.setNegativeButton( R.string.cancel, new DialogInterface.OnClickListener()
          {
            public void onClick( DialogInterface dialogInterface, int i )
            {
              dialogInterface.dismiss();
            }
          } );
          errorDialogBuilder.show();
          break;
        }
    }
  };

  private void reconnect()
  {
    if( !mConnectionProgressDialog.isShowing() )
      {
      mConnectionProgressDialog.setMessage( getString( R.string.connection_message ) );
      mConnectionProgressDialog.show();
      }

    mConnectionThread = new ConnectionThread();
    mConnectionThread.start();
  }

  private final Handler mHandler = new Handler()
  {
    @Override
    public void handleMessage( Message msg )
    {
      switch( msg.what )
        {
        case MESSAGE_RECEIVED:
          Log.i( TAG, "MESSAGE RECEIVED" );
          publishMessage( (MessageRow) msg.obj );
          break;
        case ERROR_RECEIVED:
          Log.i( TAG, "ERROR RECEIVED" );
          publishMessage( (MessageRow) msg.obj );
          break;
        }

    }

    private void publishMessage(MessageRow row)
    {
      mConversationArrayAdapter.add( row );
    }
  };

  // The action listener for the EditText widget, to listen for the return key
  private TextView.OnEditorActionListener mWriteListener = new TextView.OnEditorActionListener()
  {
    public boolean onEditorAction( TextView view, int actionId, KeyEvent event )
    {
      // If the action is a key-up event on the return key, send the message
      if( actionId == EditorInfo.IME_NULL &amp;&amp; event.getAction() == KeyEvent.ACTION_UP )
        {
        String message = view.getText().toString();
        sendMessage( message );
        }

      Log.i( TAG, "END onEditorAction" );
      return true;
    }
  };

  private class SendMessageThread extends Thread
  {
    private String message;

    public SendMessageThread( String message )
    {
      super();
      this.message = message;
    }

    @Override
    public void run()
    {
      runOnUiThread( mSendingProgressBar );
      try
        {
        weborbClient.publish( this.message );
        }
      catch( Exception e )
        {
        mConnectionHandler.obtainMessage( CONNECT_FAILED, e.getMessage() ).sendToTarget();
        }
      finally
        {
        mIsMessageSending = false;
        runOnUiThread( mSendingProgressBar );
        }
    }
  }

  private final Runnable mSendingProgressBar = new Runnable()
  {
    @Override
    public void run()
    {
      setProgressBarIndeterminateVisibility( mIsMessageSending );
    }
  };

  private void sendMessage( String message )
  {
    mIsMessageSending = true;
    new SendMessageThread( message ).start();
  }

  @Override
  public boolean onCreateOptionsMenu( Menu menu )
  {
    boolean result = super.onCreateOptionsMenu( menu );
    MenuItem menuItem = menu.add( Menu.NONE, RECONNECT, RECONNECT, R.string.menu_reconnect );
    menuItem.setIcon( android.R.drawable.ic_menu_rotate );
    menuItem = menu.add( Menu.NONE, PROPERTIES, PROPERTIES, R.string.menu_configure );
    menuItem.setIcon( android.R.drawable.ic_menu_preferences );
    return result;
  }

  @Override
  public boolean onOptionsItemSelected( MenuItem item )
  {
    boolean result = super.onOptionsItemSelected( item );
    switch( item.getItemId() )
      {
      case RECONNECT:
        reconnect();
        break;
      case PROPERTIES:
        Intent intent = new Intent();
        intent.setClass( this, ConnectionPreferencesActivity.class );
        intent.putExtra( getString(R.string.weborb_endpoint_url), weborbUrl );
        startActivityForResult( intent, CONNECTION_CONFIGURATION );
        break;
      }
    return result;
  }

  @Override
  protected void onActivityResult( int requestCode, int resultCode, Intent data )
  {
    super.onActivityResult( requestCode, resultCode, data );
    if( resultCode == ConnectionPreferencesActivity.PROPERTIES_UPDATED )
      {
      readPreferences();
      reconnect();
      }
  }

  private final Handler mExitHandler = new Handler()
  {
    @Override
    public void handleMessage( Message msg )
    {
      MainActivity.super.finish();
    }
  };

  @Override
  public void finish()
  {
    mConnectionProgressDialog.setMessage( getString( R.string.unsubscribing_message ) );
    new Thread( new Runnable()
    {
      @Override
      public void run()
      {
        if( subscription != null )
          try
            {
            subscription.unsubscribe();
            }
          catch( IOException ignored )
            {
            }
        mExitHandler.obtainMessage().sendToTarget();
      }
    } );
  }
}

class MessageArrayAdapter extends ArrayAdapter&lt;MessageRow&gt;
{
  private final int resourceId;
  private Context context;

  public MessageArrayAdapter( Context context )
  {
    super( context, R.layout.message );
    this.context = context;
    this.resourceId = R.layout.message;
  }

  @Override
  public View getView( int position, View convertView, ViewGroup parent )
  {
    View view = LayoutInflater.from( context ).inflate( resourceId, null );
    MessageRow item = getItem( position );
    ((TextView) view.findViewById( R.id.title )).setText( item.title );
    ((TextView) view.findViewById( R.id.message )).setText( item.message );
    return view;
  }
}

class MessageRow
{
  String title;
  String message;
}
</xsl:text>
                    </file>
                </folder>
            </folder>
        </folder>
        <!-- processing 'res' files -->
        <folder name="res">
            <folder name="drawable-hdpi">
                <file path="{concat($common_files_path, 'res/drawable-hdpi/icon.png')}"/>
            </folder>
            <folder name="drawable-ldpi">
                <file path="{concat($common_files_path, 'res/drawable-ldpi/icon.png')}"/>
            </folder>
            <folder name="drawable-mdpi">
                <file path="{concat($common_files_path, 'res/drawable-mdpi/icon.png')}"/>
            </folder>
            <folder name="layout">
                <file path="{concat($common_files_path, 'res/layout/main.xml')}"/>
                <file path="{concat($common_files_path, 'res/layout/message.xml')}"/>
                <file path="{concat($common_files_path, 'res/layout/preferences_layout.xml')}"/>
            </folder>
            <folder name="values">
                <file path="{concat($common_files_path, 'res/values/strings.xml')}"/>
            </folder>
        </folder>
    </xsl:template>
</xsl:stylesheet>