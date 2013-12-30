<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:codegen="urn:cogegen-xslt-lib:xslt"
    xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:template match="/">
    <xsl:param name="projectGuid" select="codegen:getGuid()"/>
    <folder name="weborb-codegen">      
        <folder name="MessagingApps">
          <folder path="messagingApplications\ios\MessagingApps.xcodeproj" />  
          <folder path="messagingApplications\ios\art" />  
          <folder name="MessagingApps">
            <folder path="messagingApplications\ios\MessagingApps\en.lproj" />
            <file path="messagingApplications\ios\MessagingApps\MessagingApps-Info.plist" />
            <file path="messagingApplications\ios\MessagingApps\MessagingApps-Prefix.pch" />
            <file path="messagingApplications\ios\MessagingApps\main.m" />
            <xsl:for-each select="data/features/feature">
              <xsl:choose>
                <xsl:when test="id = 0">
                  <folder path="messagingApplications\ios\MessagingApps\VideoBroadcast" />
                </xsl:when>
                <xsl:when test="id = 1">
                  <folder path="messagingApplications\ios\MessagingApps\VideoPLayer" />
                </xsl:when>
                <xsl:when test="id = 2">
                  <folder path="messagingApplications\ios\MessagingApps\RemoteSO" />
                  <folder path="messagingApplications\ios\MessagingApps\PhotoRSO" />
                </xsl:when>
                <xsl:when test="id = 3">
                  <folder path="messagingApplications\ios\MessagingApps\DataPush" />
                </xsl:when>
                <xsl:when test="id = 4">
                  <folder path="messagingApplications\ios\MessagingApps\MethodInvocation" />
                </xsl:when>
              </xsl:choose>
            </xsl:for-each>           
            <file path="messagingApplications\ios\MessagingApps\AppDelegate.h" />
            <xsl:call-template name="AppDelegate.m.file" /> 
          </folder>
      </folder>
    </folder>
  </xsl:template>
    
  <xsl:template name="AppDelegate.m.file">
      <file name="AppDelegate.m">//
//  AppDelegate.m
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//
          
#import "AppDelegate.h"
          <xsl:for-each select="data/features/feature">
              <xsl:choose>
                  <xsl:when test="id = 0">
#import "VideoBroadcastViewController.h"
                  </xsl:when>
                  <xsl:when test="id = 1">
#import "VideoPlayerViewController.h"
                  </xsl:when>
                  <xsl:when test="id = 2">
#import "RemoteSOViewController.h"
#import "PhotoRSOViewController.h"
                  </xsl:when>
                  <xsl:when test="id = 3">
#import "DataPushViewController.h"
                  </xsl:when>
                  <xsl:when test="id = 4">
#import "MethodInvocationViewController.h"
                  </xsl:when>
              </xsl:choose>
          </xsl:for-each>           


@implementation AppDelegate
          
@synthesize window = _window;
@synthesize tabBarController = _tabBarController;
          
-(BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
          
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
          
    BOOL iPhone = ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone);
          
    NSMutableArray *controllers = [NSMutableArray array];
          <xsl:for-each select="data/features/feature">
              <xsl:choose>
                  <xsl:when test="id = 0">
#define VIDEOBROADCAST_XIB (iPhone)?@"VideoBroadcastViewController":@"VideoBroadcastViewController_iPad"
    [controllers addObject:[[VideoBroadcastViewController alloc] initWithNibName:VIDEOBROADCAST_XIB  bundle:nil]];
                  </xsl:when>
                  <xsl:when test="id = 1">
#define VIDEOPLAYER_XIB (iPhone)?@"VideoPlayerViewController":@"VideoPlayerViewController_iPad"
    [controllers addObject:[[VideoPlayerViewController alloc] initWithNibName:VIDEOPLAYER_XIB bundle:nil]];
                  </xsl:when>
                  <xsl:when test="id = 2">
#define REMOTESO_XIB (iPhone)?@"RemoteSOViewController":@"RemoteSOViewController_iPad"
    [controllers addObject:[[RemoteSOViewController alloc] initWithNibName:REMOTESO_XIB bundle:nil]];
#define PHOTORSO_XIB (iPhone)?@"PhotoRSOViewController":@"PhotoRSOViewController_iPad"
    [controllers addObject:[[PhotoRSOViewController alloc] initWithNibName:PHOTORSO_XIB bundle:nil]];
                  </xsl:when>
                  <xsl:when test="id = 3">
#define DATAPUSH_XIB (iPhone)?@"DataPushViewController":@"DataPushViewController_iPad"
    [controllers addObject:[[DataPushViewController alloc] initWithNibName:DATAPUSH_XIB bundle:nil]];
                  </xsl:when>
                  <xsl:when test="id = 4">
#define METHODINVOCATION_XIB (iPhone)?@"MethodInvocationViewController":@"MethodInvocationViewController_iPad"
    [controllers addObject:[[MethodInvocationViewController alloc] initWithNibName:METHODINVOCATION_XIB bundle:nil]];
                  </xsl:when>
              </xsl:choose>
          </xsl:for-each>           
          
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
         
    NSString *defaultURL = @"<xsl:value-of select="data/weborbURL"/>";
    NSURL *url = [NSURL URLWithString:defaultURL];
    NSString *scheme = [url scheme];
    if ([scheme isEqualToString:@"rtmp"] || [scheme isEqualToString:@"rtmps"])
        return defaultURL;
    else 
        return [NSString stringWithFormat:@"rtmp://%@:2037", [url host]];
}
          
+(NSString *)defaultStreamURL {
          return [NSString stringWithFormat:@"%@/<xsl:value-of select="data/applicationName"/>", [AppDelegate defaultHostURL]];
}
          
+(NSString *)defaultStreamName {
    return @"iStream";
}
          
+(NSString *)defaultCallbackDemoURL {
    return [NSString stringWithFormat:@"%@/<xsl:value-of select="data/applicationName"/>", [AppDelegate defaultHostURL]];
}
          
+(NSString *)defaultClientInvokeURL {
    return [NSString stringWithFormat:@"%@/<xsl:value-of select="data/applicationName"/>", [AppDelegate defaultHostURL]];
}
          
+(NSString *)defaultSharedObjectsURL {
    return [NSString stringWithFormat:@"%@/<xsl:value-of select="data/applicationName"/>", [AppDelegate defaultHostURL]];
}
          
@end
    </file>
  </xsl:template>
</xsl:stylesheet>
