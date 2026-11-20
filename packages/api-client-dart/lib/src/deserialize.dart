import 'package:omvila_api/src/model/appointment_created.dart';
import 'package:omvila_api/src/model/appointment_detail.dart';
import 'package:omvila_api/src/model/appointment_detail_cancellation.dart';
import 'package:omvila_api/src/model/appointment_detail_customer.dart';
import 'package:omvila_api/src/model/appointment_history_entry.dart';
import 'package:omvila_api/src/model/appointment_item_request.dart';
import 'package:omvila_api/src/model/appointment_item_view.dart';
import 'package:omvila_api/src/model/booking_page_settings.dart';
import 'package:omvila_api/src/model/booking_page_settings_update.dart';
import 'package:omvila_api/src/model/booking_token_result.dart';
import 'package:omvila_api/src/model/branch.dart';
import 'package:omvila_api/src/model/branch_create.dart';
import 'package:omvila_api/src/model/branch_ref.dart';
import 'package:omvila_api/src/model/branch_service_price.dart';
import 'package:omvila_api/src/model/branch_update.dart';
import 'package:omvila_api/src/model/brand.dart';
import 'package:omvila_api/src/model/brand_settings.dart';
import 'package:omvila_api/src/model/brand_update.dart';
import 'package:omvila_api/src/model/calendar_appointment.dart';
import 'package:omvila_api/src/model/calendar_view.dart';
import 'package:omvila_api/src/model/calendar_view_days_inner.dart';
import 'package:omvila_api/src/model/calendar_view_days_inner_staff_hours_inner.dart';
import 'package:omvila_api/src/model/calendar_view_staff_inner.dart';
import 'package:omvila_api/src/model/customer_cancel_request.dart';
import 'package:omvila_api/src/model/customer_create.dart';
import 'package:omvila_api/src/model/customer_summary.dart';
import 'package:omvila_api/src/model/day_search_request.dart';
import 'package:omvila_api/src/model/day_search_result.dart';
import 'package:omvila_api/src/model/day_search_result_days_inner.dart';
import 'package:omvila_api/src/model/deposit_instructions.dart';
import 'package:omvila_api/src/model/deposit_rule.dart';
import 'package:omvila_api/src/model/deposit_view.dart';
import 'package:omvila_api/src/model/manage_appointment_create.dart';
import 'package:omvila_api/src/model/manage_appointment_create_items_inner.dart';
import 'package:omvila_api/src/model/manage_appointment_create_new_customer.dart';
import 'package:omvila_api/src/model/manage_auth_result.dart';
import 'package:omvila_api/src/model/manage_cancel_request.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates201_response.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates_request.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates_request_items_inner.dart';
import 'package:omvila_api/src/model/manage_list_branch_overrides200_response.dart';
import 'package:omvila_api/src/model/manage_list_branches200_response.dart';
import 'package:omvila_api/src/model/manage_list_customers200_response.dart';
import 'package:omvila_api/src/model/manage_list_service_categories200_response.dart';
import 'package:omvila_api/src/model/manage_list_service_templates200_response.dart';
import 'package:omvila_api/src/model/manage_list_staff200_response.dart';
import 'package:omvila_api/src/model/manage_list_time_off200_response.dart';
import 'package:omvila_api/src/model/manage_otp_request.dart';
import 'package:omvila_api/src/model/manage_record_onboarding_event_request.dart';
import 'package:omvila_api/src/model/manage_reschedule_request.dart';
import 'package:omvila_api/src/model/manage_reschedule_request_items_inner.dart';
import 'package:omvila_api/src/model/manage_session.dart';
import 'package:omvila_api/src/model/manage_session_brand.dart';
import 'package:omvila_api/src/model/manage_session_membership.dart';
import 'package:omvila_api/src/model/manage_session_user.dart';
import 'package:omvila_api/src/model/manage_set_staff_services_request.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/onboarding_checklist.dart';
import 'package:omvila_api/src/model/onboarding_checklist_steps_inner.dart';
import 'package:omvila_api/src/model/otp_challenge_response.dart';
import 'package:omvila_api/src/model/otp_verify_request.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/public_appointment.dart';
import 'package:omvila_api/src/model/public_appointment_branch.dart';
import 'package:omvila_api/src/model/public_appointment_brand.dart';
import 'package:omvila_api/src/model/public_appointment_cancellation.dart';
import 'package:omvila_api/src/model/public_appointment_create.dart';
import 'package:omvila_api/src/model/public_appointment_create_use_package_items_inner.dart';
import 'package:omvila_api/src/model/public_appointment_deposit.dart';
import 'package:omvila_api/src/model/public_appointment_items_inner.dart';
import 'package:omvila_api/src/model/public_brand.dart';
import 'package:omvila_api/src/model/public_brand_branches_inner.dart';
import 'package:omvila_api/src/model/public_brand_rating.dart';
import 'package:omvila_api/src/model/public_catalog.dart';
import 'package:omvila_api/src/model/public_catalog_categories_inner.dart';
import 'package:omvila_api/src/model/public_catalog_services_inner.dart';
import 'package:omvila_api/src/model/public_catalog_staff_inner.dart';
import 'package:omvila_api/src/model/public_catalog_staff_inner_surcharges_inner.dart';
import 'package:omvila_api/src/model/public_list_my_packages200_response.dart';
import 'package:omvila_api/src/model/public_otp_request.dart';
import 'package:omvila_api/src/model/public_otp_request_result.dart';
import 'package:omvila_api/src/model/public_reschedule_appointment_request.dart';
import 'package:omvila_api/src/model/public_search_reschedule_slots_request.dart';
import 'package:omvila_api/src/model/recurrence_request.dart';
import 'package:omvila_api/src/model/refresh_request.dart';
import 'package:omvila_api/src/model/segment_view.dart';
import 'package:omvila_api/src/model/service.dart';
import 'package:omvila_api/src/model/service_category.dart';
import 'package:omvila_api/src/model/service_category_write.dart';
import 'package:omvila_api/src/model/service_stage.dart';
import 'package:omvila_api/src/model/service_template.dart';
import 'package:omvila_api/src/model/service_write.dart';
import 'package:omvila_api/src/model/signup_request.dart';
import 'package:omvila_api/src/model/slot.dart';
import 'package:omvila_api/src/model/slot_items_inner.dart';
import 'package:omvila_api/src/model/slot_search_request.dart';
import 'package:omvila_api/src/model/slot_search_result.dart';
import 'package:omvila_api/src/model/staff_create.dart';
import 'package:omvila_api/src/model/staff_detail.dart';
import 'package:omvila_api/src/model/staff_detail_all_of_working_hours.dart';
import 'package:omvila_api/src/model/staff_service_assignment.dart';
import 'package:omvila_api/src/model/staff_summary.dart';
import 'package:omvila_api/src/model/staff_update.dart';
import 'package:omvila_api/src/model/subscription_banner.dart';
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:omvila_api/src/model/time_off.dart';
import 'package:omvila_api/src/model/time_off_create.dart';
import 'package:omvila_api/src/model/time_off_created.dart';
import 'package:omvila_api/src/model/token_pair.dart';
import 'package:omvila_api/src/model/usable_package_item.dart';
import 'package:omvila_api/src/model/weekly_hours.dart';
import 'package:omvila_api/src/model/weekly_hours_days_inner.dart';
import 'package:omvila_api/src/model/working_hours_override.dart';
import 'package:omvila_api/src/model/working_hours_override_create.dart';

final _regList = RegExp(r'^List<(.*)>$');
final _regSet = RegExp(r'^Set<(.*)>$');
final _regMap = RegExp(r'^Map<String,(.*)>$');

  ReturnType deserialize<ReturnType, BaseType>(dynamic value, String targetType, {bool growable= true}) {
      switch (targetType) {
        case 'String':
          return '$value' as ReturnType;
        case 'int':
          return (value is int ? value : int.parse('$value')) as ReturnType;
        case 'bool':
          if (value is bool) {
            return value as ReturnType;
          }
          final valueString = '$value'.toLowerCase();
          return (valueString == 'true' || valueString == '1') as ReturnType;
        case 'double':
          return (value is double ? value : double.parse('$value')) as ReturnType;
        case 'AppointmentChannel':
          
          
        case 'AppointmentCreated':
          return AppointmentCreated.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentDetail':
          return AppointmentDetail.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentDetailCancellation':
          return AppointmentDetailCancellation.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentDetailCustomer':
          return AppointmentDetailCustomer.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentHistoryEntry':
          return AppointmentHistoryEntry.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentItemRequest':
          return AppointmentItemRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentItemView':
          return AppointmentItemView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'AppointmentStatus':
          
          
        case 'BookingPageSettings':
          return BookingPageSettings.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BookingPageSettingsUpdate':
          return BookingPageSettingsUpdate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BookingTokenResult':
          return BookingTokenResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Branch':
          return Branch.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BranchCreate':
          return BranchCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BranchRef':
          return BranchRef.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BranchServicePrice':
          return BranchServicePrice.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BranchUpdate':
          return BranchUpdate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Brand':
          return Brand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BrandSettings':
          return BrandSettings.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BrandUpdate':
          return BrandUpdate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CalendarAppointment':
          return CalendarAppointment.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CalendarView':
          return CalendarView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CalendarViewDaysInner':
          return CalendarViewDaysInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CalendarViewDaysInnerStaffHoursInner':
          return CalendarViewDaysInnerStaffHoursInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CalendarViewStaffInner':
          return CalendarViewStaffInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CancellationReason':
          
          
        case 'CustomerCancelRequest':
          return CustomerCancelRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerCreate':
          return CustomerCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerSummary':
          return CustomerSummary.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchRequest':
          return DaySearchRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchResult':
          return DaySearchResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchResultDaysInner':
          return DaySearchResultDaysInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositInstructions':
          return DepositInstructions.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositRule':
          return DepositRule.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositView':
          return DepositView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DisplayStatus':
          
          
        case 'ErrorCode':
          
          
        case 'IndustryCode':
          
          
        case 'ManageAppointmentCreate':
          return ManageAppointmentCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAppointmentCreateItemsInner':
          return ManageAppointmentCreateItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAppointmentCreateNewCustomer':
          return ManageAppointmentCreateNewCustomer.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAuthResult':
          return ManageAuthResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCancelRequest':
          return ManageCancelRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCreateServicesFromTemplates201Response':
          return ManageCreateServicesFromTemplates201Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCreateServicesFromTemplatesRequest':
          return ManageCreateServicesFromTemplatesRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCreateServicesFromTemplatesRequestItemsInner':
          return ManageCreateServicesFromTemplatesRequestItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListBranchOverrides200Response':
          return ManageListBranchOverrides200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListBranches200Response':
          return ManageListBranches200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListCustomers200Response':
          return ManageListCustomers200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListServiceCategories200Response':
          return ManageListServiceCategories200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListServiceTemplates200Response':
          return ManageListServiceTemplates200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListStaff200Response':
          return ManageListStaff200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageListTimeOff200Response':
          return ManageListTimeOff200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageOtpRequest':
          return ManageOtpRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageRecordOnboardingEventRequest':
          return ManageRecordOnboardingEventRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageRescheduleRequest':
          return ManageRescheduleRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageRescheduleRequestItemsInner':
          return ManageRescheduleRequestItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSession':
          return ManageSession.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSessionBrand':
          return ManageSessionBrand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSessionMembership':
          return ManageSessionMembership.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSessionUser':
          return ManageSessionUser.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSetStaffServicesRequest':
          return ManageSetStaffServicesRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Money':
          return Money.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OnboardingChecklist':
          return OnboardingChecklist.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OnboardingChecklistStepsInner':
          return OnboardingChecklistStepsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OtpChallengeResponse':
          return OtpChallengeResponse.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OtpVerifyRequest':
          return OtpVerifyRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Problem':
          return Problem.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointment':
          return PublicAppointment.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentBranch':
          return PublicAppointmentBranch.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentBrand':
          return PublicAppointmentBrand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentCancellation':
          return PublicAppointmentCancellation.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentCreate':
          return PublicAppointmentCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentCreateUsePackageItemsInner':
          return PublicAppointmentCreateUsePackageItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentDeposit':
          return PublicAppointmentDeposit.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicAppointmentItemsInner':
          return PublicAppointmentItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicBrand':
          return PublicBrand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicBrandBranchesInner':
          return PublicBrandBranchesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicBrandRating':
          return PublicBrandRating.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicCatalog':
          return PublicCatalog.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicCatalogCategoriesInner':
          return PublicCatalogCategoriesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicCatalogServicesInner':
          return PublicCatalogServicesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicCatalogStaffInner':
          return PublicCatalogStaffInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicCatalogStaffInnerSurchargesInner':
          return PublicCatalogStaffInnerSurchargesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicListMyPackages200Response':
          return PublicListMyPackages200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicOtpRequest':
          return PublicOtpRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicOtpRequestResult':
          return PublicOtpRequestResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicRescheduleAppointmentRequest':
          return PublicRescheduleAppointmentRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicSearchRescheduleSlotsRequest':
          return PublicSearchRescheduleSlotsRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'RecurrenceRequest':
          return RecurrenceRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'RefreshRequest':
          return RefreshRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SegmentView':
          return SegmentView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Service':
          return Service.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ServiceCategory':
          return ServiceCategory.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ServiceCategoryWrite':
          return ServiceCategoryWrite.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ServiceStage':
          return ServiceStage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ServiceTemplate':
          return ServiceTemplate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ServiceWrite':
          return ServiceWrite.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SignupRequest':
          return SignupRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Slot':
          return Slot.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SlotItemsInner':
          return SlotItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SlotSearchRequest':
          return SlotSearchRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SlotSearchResult':
          return SlotSearchResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffCreate':
          return StaffCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffDetail':
          return StaffDetail.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffDetailAllOfWorkingHours':
          return StaffDetailAllOfWorkingHours.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffServiceAssignment':
          return StaffServiceAssignment.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffSummary':
          return StaffSummary.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffUpdate':
          return StaffUpdate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SubscriptionBanner':
          return SubscriptionBanner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'TimeInterval':
          return TimeInterval.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'TimeOff':
          return TimeOff.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'TimeOffCreate':
          return TimeOffCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'TimeOffCreated':
          return TimeOffCreated.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'TokenPair':
          return TokenPair.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'UsablePackageItem':
          return UsablePackageItem.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WeeklyHours':
          return WeeklyHours.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WeeklyHoursDaysInner':
          return WeeklyHoursDaysInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WorkingHoursOverride':
          return WorkingHoursOverride.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WorkingHoursOverrideCreate':
          return WorkingHoursOverrideCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        default:
          RegExpMatch? match;

          if (value is List && (match = _regList.firstMatch(targetType)) != null) {
            targetType = match![1]!; // ignore: parameter_assignments
            return value
              .map<BaseType>((dynamic v) => deserialize<BaseType, BaseType>(v, targetType, growable: growable))
              .toList(growable: growable) as ReturnType;
          }
          if (value is Set && (match = _regSet.firstMatch(targetType)) != null) {
            targetType = match![1]!; // ignore: parameter_assignments
            return value
              .map<BaseType>((dynamic v) => deserialize<BaseType, BaseType>(v, targetType, growable: growable))
              .toSet() as ReturnType;
          }
          if (value is Map && (match = _regMap.firstMatch(targetType)) != null) {
            targetType = match![1]!.trim(); // ignore: parameter_assignments
            return Map<String, BaseType>.fromIterables(
              value.keys as Iterable<String>,
              value.values.map((dynamic v) => deserialize<BaseType, BaseType>(v, targetType, growable: growable)),
            ) as ReturnType;
          }
          break;
    }
    throw Exception('Cannot deserialize');
  }