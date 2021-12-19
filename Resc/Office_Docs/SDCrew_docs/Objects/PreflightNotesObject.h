//
//  PreflightNotesObject.h
//  SDCrew
//
//  Created by Roman Gazi on 23/5/21.
//  Copyright © 2021 brotecs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "YapDatabaseObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface PreflightNotesObject : YapDatabaseObject

@property (nonatomic, strong, nullable) NSString *calendarNoteId;
@property (nonatomic, strong, nullable) NSString *note;
@property (nonatomic, strong, nullable) NSString *customerId;
@property (nonatomic, strong, nullable) NSDate *calendarDate;
@property (nonatomic, strong, nullable) NSString *createdByUser;
@property (nonatomic, strong, nullable) NSDate *createdDate;
@property (nonatomic, strong, nullable) NSString *modifiedByUser;
@property (nonatomic, strong, nullable) NSDate *modifiedDate;
@property (nonatomic) BOOL isUpdated;

-(PreflightNotesObject *)duplicate;
@end

NS_ASSUME_NONNULL_END
