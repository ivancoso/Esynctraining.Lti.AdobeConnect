//
//  AppDelegate.m
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "AppDelegate.h"

@implementation AppDelegate

@synthesize window = _window;

-(BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    return YES;
}
							
-(void)applicationWillResignActive:(UIApplication *)application {
}

-(void)applicationDidEnterBackground:(UIApplication *)application {
}

-(void)applicationWillEnterForeground:(UIApplication *)application {
}

-(void)applicationDidBecomeActive:(UIApplication *)application {
}

-(void)applicationWillTerminate:(UIApplication *)application {
}

+(NSString *)defaultURL {
    //return @"rtmp://10.0.1.141:2037/root";
    return@"http://10.0.1.141/weborb5/weborb.aspx";
}

+(NSString *)defaultDestinationName {
    return @"WdmfMessagingDestination";
}

@end
