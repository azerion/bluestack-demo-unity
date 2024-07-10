#import <Foundation/Foundation.h>

/// Base type representing a GADU* pointer.
typedef const void *GADUTypeRef;

typedef const void *GADUTypeMobileAdsClientRef;

/// Type representing a GADUTypeRequestConfigurationRef
typedef const void *GADUTypeRequestConfigurationRef;

/// Type representing a AdError type
typedef const void *GADUTypeErrorRef;

/// Type representing a NSMutableDictionary of extras.
typedef const void *GADUTypeMutableDictionaryRef;


// MARK: - GADUAdInspector

/// Callback when ad inspector UI closes.
typedef void (*GADUAdInspectorCompleteCallback)(GADUTypeMobileAdsClientRef *clientRef,
                                                const char *error);
