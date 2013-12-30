//
//  DataPushViewController.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "DataPushViewController.h"
#import "AppDelegate.h"
#import "DEBUG.h"

#define STATUS_SUCCESS_RESULT 0x02
#define HELP_HTML @"<html><head><style type='text/css'>body {font-family: Helvetica; font-size: 50px;}</style></head><body><p>INSTRUCTIONS: This example requires special code on the server-side which will push data into the client code by invoking client-side functions. In order to deploy the server-side code, make sure that your application adapter either is or extends from Weborb.Examples.RTMPDemo.AppHandler. See the <a href='http://www.themidnightcoders.com/rtmpdemosetup'>product documentation</a> for additional instructions.</p></body></html>"

@interface DataPushViewController ()

@end

@implementation DataPushViewController

#pragma mark -
#pragma mark  View lifecycle

-(id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
        self.title = @"Data Push";
        self.tabBarItem.image = [UIImage imageNamed:@"datapush"];
    }
    return self;
}

-(void)viewDidLoad {
    
    [super viewDidLoad];
	
    socket = nil;
    alerts = 100;
    
    hostTextField.text = [AppDelegate defaultCallbackDemoURL];
    hostTextField.delegate = self;

	helpWebView.delegate = self;
	helpWebView.scalesPageToFit = YES;
	helpWebView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
	
	netActivity = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
	netActivity.center = CGPointMake(155.0f, 120.0f);
	[helpWebView addSubview:netActivity];
    
    [helpWebView loadHTMLString:HELP_HTML baseURL:NULL];

    [DebLog setIsActive:YES];
}

-(void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

-(BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark -
#pragma mark Private Methods 

-(void)showAlert:(NSString *)message {
    UIAlertView *av = [[UIAlertView alloc] initWithTitle:@"Receive" message:message delegate:self cancelButtonTitle:@"Ok" otherButtonTitles:nil];
	alerts++;
	av.tag = alerts;
    [av show];
}

-(void)socketConnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = YES;
    helpWebView.hidden = YES;
    noteTextView.hidden = NO;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Disconnect";     
    
}

-(void)socketDisconnected {
    
    [netActivity stopAnimating];
    
    hostTextField.hidden = NO;    
    helpWebView.hidden = NO;
    noteTextView.hidden = YES;
    btnConnect.enabled = YES;     
    btnConnect.title = @"Connect";     
}

-(void)doConnect {				
    
    socket = [[RTMPClient alloc] init:hostTextField.text];
    socket.delegate = self;
    [socket connect];
    
    btnConnect.enabled = NO;     
    
    [netActivity startAnimating];
}

-(void)doDisconnect {	
    
    [socket disconnect];
    socket = nil;
    
    [self socketDisconnected];
}


#pragma mark -
#pragma mark Public Methods 

// ACTIONS

-(IBAction)connectControl:(id)sender {
    (!socket) ? [self doConnect] : [self doDisconnect];
}

#pragma mark -
#pragma mark UITextFieldDelegate Methods 

-(BOOL)textFieldShouldReturn:(UITextField *)textField {
	[textField resignFirstResponder];
	return YES;
}

#pragma mark -
#pragma mark UIWebView Delegate Methods 

-(void)webViewDidStartLoad:(UIWebView *)webView {
	[netActivity startAnimating];
}

-(void)webViewDidFinishLoad:(UIWebView *)webView {
	[netActivity stopAnimating];
}


#pragma mark -
#pragma mark IRTMPClientDelegate Methods 

-(void)connectedEvent {
    [self socketConnected];
}

-(void)disconnectedEvent {	
    [self doDisconnect];
 	[self showAlert:[NSString stringWithString:@"Disconnected"]];   
}

-(void)connectFailedEvent:(int)code description:(NSString *)description {
    
    [self doDisconnect];
    
    [self showAlert:(code == -1) ? 
     [NSString stringWithFormat:@"Unable to connect to the server. Make sure the hostname/IP address and port number are valid\n"] : 
     [NSString stringWithFormat:@"connectFailedEvent: %@ \n", description]];    
}

-(void)resultReceived:(id <IServiceCall>)call {
    
    NSArray *args = [call getArguments];    
    if (!args.count || ([call getStatus] != STATUS_SUCCESS_RESULT)) // this call is not a server invoke
        return;
        
    noteTextView.text = [NSString stringWithFormat:@"%@%@:%@\n", noteTextView.text, [call getServiceMethodName], [args objectAtIndex:0]];
}


@end
