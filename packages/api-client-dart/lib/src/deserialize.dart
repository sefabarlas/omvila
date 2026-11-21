import 'package:omvila_api/src/model/appointment_created.dart';
import 'package:omvila_api/src/model/appointment_detail.dart';
import 'package:omvila_api/src/model/appointment_detail_cancellation.dart';
import 'package:omvila_api/src/model/appointment_detail_customer.dart';
import 'package:omvila_api/src/model/appointment_history_entry.dart';
import 'package:omvila_api/src/model/appointment_item_request.dart';
import 'package:omvila_api/src/model/appointment_item_view.dart';
import 'package:omvila_api/src/model/bank_account.dart';
import 'package:omvila_api/src/model/bank_account_write.dart';
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
import 'package:omvila_api/src/model/cash_day.dart';
import 'package:omvila_api/src/model/cash_day_close_request.dart';
import 'package:omvila_api/src/model/cash_day_closing.dart';
import 'package:omvila_api/src/model/cash_day_transfer.dart';
import 'package:omvila_api/src/model/cash_method_total.dart';
import 'package:omvila_api/src/model/completion_payment.dart';
import 'package:omvila_api/src/model/customer_appointment_row.dart';
import 'package:omvila_api/src/model/customer_cancel_request.dart';
import 'package:omvila_api/src/model/customer_create.dart';
import 'package:omvila_api/src/model/customer_deletion_preview.dart';
import 'package:omvila_api/src/model/customer_deletion_preview_active_packages_inner.dart';
import 'package:omvila_api/src/model/customer_deletion_request.dart';
import 'package:omvila_api/src/model/customer_import_commit_request.dart';
import 'package:omvila_api/src/model/customer_import_preview.dart';
import 'package:omvila_api/src/model/customer_import_result.dart';
import 'package:omvila_api/src/model/customer_import_row_error.dart';
import 'package:omvila_api/src/model/customer_merge_request.dart';
import 'package:omvila_api/src/model/customer_package.dart';
import 'package:omvila_api/src/model/customer_package_item_view.dart';
import 'package:omvila_api/src/model/customer_package_page.dart';
import 'package:omvila_api/src/model/customer_payment_page.dart';
import 'package:omvila_api/src/model/customer_ref.dart';
import 'package:omvila_api/src/model/customer_review_page.dart';
import 'package:omvila_api/src/model/customer_summary.dart';
import 'package:omvila_api/src/model/data_deletion_request_page.dart';
import 'package:omvila_api/src/model/data_deletion_request_view.dart';
import 'package:omvila_api/src/model/day_search_request.dart';
import 'package:omvila_api/src/model/day_search_result.dart';
import 'package:omvila_api/src/model/day_search_result_days_inner.dart';
import 'package:omvila_api/src/model/deposit_instructions.dart';
import 'package:omvila_api/src/model/deposit_refund_request.dart';
import 'package:omvila_api/src/model/deposit_rule.dart';
import 'package:omvila_api/src/model/deposit_view.dart';
import 'package:omvila_api/src/model/device_registration.dart';
import 'package:omvila_api/src/model/in_app_notification.dart';
import 'package:omvila_api/src/model/in_app_notification_page.dart';
import 'package:omvila_api/src/model/manage_appointment_create.dart';
import 'package:omvila_api/src/model/manage_appointment_create_items_inner.dart';
import 'package:omvila_api/src/model/manage_appointment_create_new_customer.dart';
import 'package:omvila_api/src/model/manage_approve_request.dart';
import 'package:omvila_api/src/model/manage_auth_result.dart';
import 'package:omvila_api/src/model/manage_cancel_request.dart';
import 'package:omvila_api/src/model/manage_complete_request.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates201_response.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates_request.dart';
import 'package:omvila_api/src/model/manage_create_services_from_templates_request_items_inner.dart';
import 'package:omvila_api/src/model/manage_list_branch_overrides200_response.dart';
import 'package:omvila_api/src/model/manage_list_branches200_response.dart';
import 'package:omvila_api/src/model/manage_list_customer_appointments200_response.dart';
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
import 'package:omvila_api/src/model/manage_session_sms_credits.dart';
import 'package:omvila_api/src/model/manage_session_user.dart';
import 'package:omvila_api/src/model/manage_set_staff_services_request.dart';
import 'package:omvila_api/src/model/manage_waitlist_create.dart';
import 'package:omvila_api/src/model/mark_notifications_read_request.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/my_notification_preferences.dart';
import 'package:omvila_api/src/model/notification_settings.dart';
import 'package:omvila_api/src/model/notification_settings_update.dart';
import 'package:omvila_api/src/model/notification_settings_update_templates_inner.dart';
import 'package:omvila_api/src/model/notification_target.dart';
import 'package:omvila_api/src/model/notification_template_setting.dart';
import 'package:omvila_api/src/model/onboarding_checklist.dart';
import 'package:omvila_api/src/model/onboarding_checklist_steps_inner.dart';
import 'package:omvila_api/src/model/otp_challenge_response.dart';
import 'package:omvila_api/src/model/otp_verify_request.dart';
import 'package:omvila_api/src/model/package_definition.dart';
import 'package:omvila_api/src/model/package_definition_items_inner.dart';
import 'package:omvila_api/src/model/package_item_write.dart';
import 'package:omvila_api/src/model/package_list.dart';
import 'package:omvila_api/src/model/package_overview.dart';
import 'package:omvila_api/src/model/package_redemption_request.dart';
import 'package:omvila_api/src/model/package_refund_request.dart';
import 'package:omvila_api/src/model/package_sale_request.dart';
import 'package:omvila_api/src/model/package_session_usage_view.dart';
import 'package:omvila_api/src/model/package_write.dart';
import 'package:omvila_api/src/model/payment_view.dart';
import 'package:omvila_api/src/model/pending_approval_item.dart';
import 'package:omvila_api/src/model/pending_approval_list.dart';
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
import 'package:omvila_api/src/model/public_review_create.dart';
import 'package:omvila_api/src/model/public_review_invite.dart';
import 'package:omvila_api/src/model/public_review_invite_brand.dart';
import 'package:omvila_api/src/model/public_review_invite_review.dart';
import 'package:omvila_api/src/model/public_review_invite_staff.dart';
import 'package:omvila_api/src/model/public_search_reschedule_slots_request.dart';
import 'package:omvila_api/src/model/public_waitlist_entry.dart';
import 'package:omvila_api/src/model/public_waitlist_join.dart';
import 'package:omvila_api/src/model/public_waitlist_offer.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_branch.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_brand.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_items_inner.dart';
import 'package:omvila_api/src/model/recurrence_request.dart';
import 'package:omvila_api/src/model/refresh_request.dart';
import 'package:omvila_api/src/model/registered_device.dart';
import 'package:omvila_api/src/model/review_overview.dart';
import 'package:omvila_api/src/model/review_overview_top_staff.dart';
import 'package:omvila_api/src/model/review_page.dart';
import 'package:omvila_api/src/model/review_reply_request.dart';
import 'package:omvila_api/src/model/review_view.dart';
import 'package:omvila_api/src/model/review_view_reply.dart';
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
import 'package:omvila_api/src/model/sms_overview.dart';
import 'package:omvila_api/src/model/sms_overview_sent_this_month.dart';
import 'package:omvila_api/src/model/sms_sender.dart';
import 'package:omvila_api/src/model/sms_sender_request.dart';
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
import 'package:omvila_api/src/model/unread_notification_count.dart';
import 'package:omvila_api/src/model/usable_package_item.dart';
import 'package:omvila_api/src/model/waitlist_entry_view.dart';
import 'package:omvila_api/src/model/waitlist_list.dart';
import 'package:omvila_api/src/model/waitlist_offer_request.dart';
import 'package:omvila_api/src/model/waitlist_offer_view.dart';
import 'package:omvila_api/src/model/waitlist_overview.dart';
import 'package:omvila_api/src/model/waitlist_preferences.dart';
import 'package:omvila_api/src/model/walk_in_create.dart';
import 'package:omvila_api/src/model/walk_in_queue.dart';
import 'package:omvila_api/src/model/walk_in_start_request.dart';
import 'package:omvila_api/src/model/walk_in_started.dart';
import 'package:omvila_api/src/model/walk_in_ticket_view.dart';
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
          
          
        case 'BankAccount':
          return BankAccount.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'BankAccountWrite':
          return BankAccountWrite.fromJson(value as Map<String, dynamic>) as ReturnType;
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
          
          
        case 'CashDay':
          return CashDay.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CashDayCloseRequest':
          return CashDayCloseRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CashDayClosing':
          return CashDayClosing.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CashDayTransfer':
          return CashDayTransfer.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CashMethodTotal':
          return CashMethodTotal.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CollectedPaymentMethod':
          
          
        case 'CompletionPayment':
          return CompletionPayment.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerAppointmentRow':
          return CustomerAppointmentRow.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerCancelRequest':
          return CustomerCancelRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerCreate':
          return CustomerCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerDeletionPreview':
          return CustomerDeletionPreview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerDeletionPreviewActivePackagesInner':
          return CustomerDeletionPreviewActivePackagesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerDeletionRequest':
          return CustomerDeletionRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerImportCommitRequest':
          return CustomerImportCommitRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerImportPreview':
          return CustomerImportPreview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerImportResult':
          return CustomerImportResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerImportRowError':
          return CustomerImportRowError.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerMergeRequest':
          return CustomerMergeRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerNotificationEvent':
          
          
        case 'CustomerPackage':
          return CustomerPackage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerPackageItemView':
          return CustomerPackageItemView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerPackagePage':
          return CustomerPackagePage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerPackageStatus':
          
          
        case 'CustomerPaymentPage':
          return CustomerPaymentPage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerRef':
          return CustomerRef.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerReviewPage':
          return CustomerReviewPage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'CustomerSummary':
          return CustomerSummary.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DataDeletionRequestPage':
          return DataDeletionRequestPage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DataDeletionRequestView':
          return DataDeletionRequestView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchRequest':
          return DaySearchRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchResult':
          return DaySearchResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DaySearchResultDaysInner':
          return DaySearchResultDaysInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositInstructions':
          return DepositInstructions.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositRefundRequest':
          return DepositRefundRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositRule':
          return DepositRule.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DepositStatus':
          
          
        case 'DepositView':
          return DepositView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DevicePlatform':
          
          
        case 'DeviceRegistration':
          return DeviceRegistration.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'DisplayStatus':
          
          
        case 'ErrorCode':
          
          
        case 'InAppNotification':
          return InAppNotification.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'InAppNotificationPage':
          return InAppNotificationPage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'IndustryCode':
          
          
        case 'ManageAppointmentCreate':
          return ManageAppointmentCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAppointmentCreateItemsInner':
          return ManageAppointmentCreateItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAppointmentCreateNewCustomer':
          return ManageAppointmentCreateNewCustomer.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageApproveRequest':
          return ManageApproveRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageAuthResult':
          return ManageAuthResult.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCancelRequest':
          return ManageCancelRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageCompleteRequest':
          return ManageCompleteRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
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
        case 'ManageListCustomerAppointments200Response':
          return ManageListCustomerAppointments200Response.fromJson(value as Map<String, dynamic>) as ReturnType;
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
        case 'ManageSessionSmsCredits':
          return ManageSessionSmsCredits.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSessionUser':
          return ManageSessionUser.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageSetStaffServicesRequest':
          return ManageSetStaffServicesRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ManageWaitlistCreate':
          return ManageWaitlistCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'MarkNotificationsReadRequest':
          return MarkNotificationsReadRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'Money':
          return Money.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'MyNotificationPreferences':
          return MyNotificationPreferences.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'NotificationSettings':
          return NotificationSettings.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'NotificationSettingsUpdate':
          return NotificationSettingsUpdate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'NotificationSettingsUpdateTemplatesInner':
          return NotificationSettingsUpdateTemplatesInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'NotificationTarget':
          return NotificationTarget.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'NotificationTemplateSetting':
          return NotificationTemplateSetting.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OnboardingChecklist':
          return OnboardingChecklist.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OnboardingChecklistStepsInner':
          return OnboardingChecklistStepsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OtpChallengeResponse':
          return OtpChallengeResponse.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'OtpVerifyRequest':
          return OtpVerifyRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageDefinition':
          return PackageDefinition.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageDefinitionItemsInner':
          return PackageDefinitionItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageItemWrite':
          return PackageItemWrite.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageList':
          return PackageList.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageOverview':
          return PackageOverview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageRedemptionRequest':
          return PackageRedemptionRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageRefundRequest':
          return PackageRefundRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageSaleRequest':
          return PackageSaleRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageSessionUsageView':
          return PackageSessionUsageView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PackageWrite':
          return PackageWrite.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PaymentMethod':
          
          
        case 'PaymentPurpose':
          
          
        case 'PaymentStatus':
          
          
        case 'PaymentView':
          return PaymentView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PendingApprovalItem':
          return PendingApprovalItem.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PendingApprovalList':
          return PendingApprovalList.fromJson(value as Map<String, dynamic>) as ReturnType;
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
        case 'PublicReviewCreate':
          return PublicReviewCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicReviewInvite':
          return PublicReviewInvite.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicReviewInviteBrand':
          return PublicReviewInviteBrand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicReviewInviteReview':
          return PublicReviewInviteReview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicReviewInviteStaff':
          return PublicReviewInviteStaff.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicSearchRescheduleSlotsRequest':
          return PublicSearchRescheduleSlotsRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistEntry':
          return PublicWaitlistEntry.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistJoin':
          return PublicWaitlistJoin.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistOffer':
          return PublicWaitlistOffer.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistOfferBranch':
          return PublicWaitlistOfferBranch.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistOfferBrand':
          return PublicWaitlistOfferBrand.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'PublicWaitlistOfferItemsInner':
          return PublicWaitlistOfferItemsInner.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'RecurrenceRequest':
          return RecurrenceRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'RefreshRequest':
          return RefreshRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'RegisteredDevice':
          return RegisteredDevice.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewOverview':
          return ReviewOverview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewOverviewTopStaff':
          return ReviewOverviewTopStaff.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewPage':
          return ReviewPage.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewReplyRequest':
          return ReviewReplyRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewView':
          return ReviewView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'ReviewViewReply':
          return ReviewViewReply.fromJson(value as Map<String, dynamic>) as ReturnType;
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
        case 'SmsOverview':
          return SmsOverview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SmsOverviewSentThisMonth':
          return SmsOverviewSentThisMonth.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SmsSender':
          return SmsSender.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SmsSenderRequest':
          return SmsSenderRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'SmsSenderStatus':
          
          
        case 'StaffCreate':
          return StaffCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffDetail':
          return StaffDetail.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffDetailAllOfWorkingHours':
          return StaffDetailAllOfWorkingHours.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'StaffNotificationEvent':
          
          
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
        case 'UnreadNotificationCount':
          return UnreadNotificationCount.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'UsablePackageItem':
          return UsablePackageItem.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistEntryStatus':
          
          
        case 'WaitlistEntryView':
          return WaitlistEntryView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistList':
          return WaitlistList.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistOfferRequest':
          return WaitlistOfferRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistOfferStatus':
          
          
        case 'WaitlistOfferView':
          return WaitlistOfferView.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistOverview':
          return WaitlistOverview.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistPreferences':
          return WaitlistPreferences.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WaitlistTimeWindow':
          
          
        case 'WalkInCreate':
          return WalkInCreate.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WalkInQueue':
          return WalkInQueue.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WalkInStartRequest':
          return WalkInStartRequest.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WalkInStarted':
          return WalkInStarted.fromJson(value as Map<String, dynamic>) as ReturnType;
        case 'WalkInTicketStatus':
          
          
        case 'WalkInTicketView':
          return WalkInTicketView.fromJson(value as Map<String, dynamic>) as ReturnType;
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