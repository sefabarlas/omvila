//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Platform izin kodu (docs/alan/roller-yetkiler.md § Platform rolleri)
enum AdminPermission {
  @JsonValue(r'platform.brands.view')
  platformPeriodBrandsPeriodView(r'platform.brands.view'),
  @JsonValue(r'platform.brands.manage')
  platformPeriodBrandsPeriodManage(r'platform.brands.manage'),
  @JsonValue(r'platform.payments.match')
  platformPeriodPaymentsPeriodMatch(r'platform.payments.match'),
  @JsonValue(r'platform.sms-senders.decide')
  platformPeriodSmsSendersPeriodDecide(r'platform.sms-senders.decide'),
  @JsonValue(r'platform.credits.grant')
  platformPeriodCreditsPeriodGrant(r'platform.credits.grant'),
  @JsonValue(r'platform.plans.view')
  platformPeriodPlansPeriodView(r'platform.plans.view'),
  @JsonValue(r'platform.plans.manage')
  platformPeriodPlansPeriodManage(r'platform.plans.manage'),
  @JsonValue(r'platform.billing.manage')
  platformPeriodBillingPeriodManage(r'platform.billing.manage'),
  @JsonValue(r'platform.audit.view')
  platformPeriodAuditPeriodView(r'platform.audit.view'),
  @JsonValue(r'platform.audit.export')
  platformPeriodAuditPeriodExport(r'platform.audit.export'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const AdminPermission(this.value);

  final String value;

  @override
  String toString() => value;
}
