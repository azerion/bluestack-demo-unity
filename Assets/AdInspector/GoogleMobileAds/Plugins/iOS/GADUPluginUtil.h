#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADUTypes.h"

/// Returns YES if the operating system is at least the supplied major version.
BOOL GADUIsOperatingSystemAtLeastVersion(NSInteger majorVersion);


@interface GADUPluginUtil : NSObject

/// Whether the Unity app should be paused when a full screen ad is displayed.
//@property(class) BOOL pauseOnBackground;

/// Returns an NSString copying the characters from |bytes|, a C array of UTF8-encoded bytes.
/// Returns nil if |bytes| is NULL.
+ (NSString *)GADUStringFromUTF8String:(const char *)bytes;

/// Returns the Unity view controller.
+ (UIViewController *)unityGLViewController;

@end
