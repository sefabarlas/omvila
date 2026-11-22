//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Salon rolü (İşletme sahibi, Şube yöneticisi, Personel)
enum MembershipRole {
  @JsonValue(r'Owner')
  owner(r'Owner'),
  @JsonValue(r'BranchManager')
  branchManager(r'BranchManager'),
  @JsonValue(r'Staff')
  staff(r'Staff'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MembershipRole(this.value);

  final String value;

  @override
  String toString() => value;
}
