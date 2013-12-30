//
//  AppDelegate.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import "AppDelegate.h"
#import "VideoBroadcastViewController.h"
#import "VideoPlayerViewController.h"
#import "RemoteSOViewController.h"
#import "PhotoRSOViewController.h"
#import "DataPushViewController.h"
#import "MethodInvocationViewController.h"

@implementation AppDelegate

@synthesize window = _window;
@synthesize tabBarController = _tabBarController;

-(BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    
    BOOL iPhone = ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone);
    
    NSMutableArray *controllers = [NSMutableArray array];
#define VIDEOBROADCAST_XIB (iPhone)?@"VideoBroadcastViewController":@"VideoBroadcastViewController_iPad"
    [controllers addObject:[[VideoBroadcastViewController alloc] initWithNibName:VIDEOBROADCAST_XIB  bundle:nil]];
#define VIDEOPLAYER_XIB (iPhone)?@"VideoPlayerViewController":@"VideoPlayerViewController_iPad"
    [controllers addObject:[[VideoPlayerViewController alloc] initWithNibName:VIDEOPLAYER_XIB bundle:nil]];
#define REMOTESO_XIB (iPhone)?@"RemoteSOViewController":@"RemoteSOViewController_iPad"
    [controllers addObject:[[RemoteSOViewController alloc] initWithNibName:REMOTESO_XIB bundle:nil]];
#define PHOTORSO_XIB (iPhone)?@"PhotoRSOViewController":@"PhotoRSOViewController_iPad"
    [controllers addObject:[[PhotoRSOViewController alloc] initWithNibName:PHOTORSO_XIB bundle:nil]];
#define DATAPUSH_XIB (iPhone)?@"DataPushViewController":@"DataPushViewController_iPad"
    [controllers addObject:[[DataPushViewController alloc] initWithNibName:DATAPUSH_XIB bundle:nil]];
#define METHODINVOCATION_XIB (iPhone)?@"MethodInvocationViewController":@"MethodInvocationViewController_iPad"
    [controllers addObject:[[MethodInvocationViewController alloc] initWithNibName:METHODINVOCATION_XIB bundle:nil]];

    self.tabBarController = [[UITabBarController alloc] init];
    self.tabBarController.viewControllers = controllers;
    self.window.rootViewController = self.tabBarController;
    
    self.window.backgroundColor = [UIColor whiteColor];
    [self.window makeKeyAndVisible];
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

+(NSString *)defaultHostURL {
    return @"rtmp://examples.themidnightcoders.com:2037";
}

+(NSString *)defaultStreamURL {
    return @"rtmp://192.168.2.105:1935/live";
    return [NSString stringWithFormat:@"%@/live", [AppDelegate defaultHostURL]];
}

+(NSString *)defaultStreamName {
    return @"iStream";
}

+(NSString *)defaultCallbackDemoURL {
    return [NSString stringWithFormat:@"%@/CallbackDemo", [AppDelegate defaultHostURL]];
}

+(NSString *)defaultClientInvokeURL {
    return [NSString stringWithFormat:@"%@/ClientInvoke", [AppDelegate defaultHostURL]];
}

+(NSString *)defaultSharedObjectsURL {
    return [NSString stringWithFormat:@"%@/SharedObjectsApp", [AppDelegate defaultHostURL]];
}

@end
