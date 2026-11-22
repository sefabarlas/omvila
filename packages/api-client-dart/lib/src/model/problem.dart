//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/error_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'problem.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Problem {
  /// Returns a new [Problem] instance.
  Problem({

    required  this.type,

    required  this.title,

    required  this.status,

    required  this.code,

     this.detail,

     this.traceId,

     this.errors,

     this.meta,
  });

  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  )


  final String type;



  @JsonKey(
    
    name: r'title',
    required: true,
    includeIfNull: false,
  )


  final String title;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  )


  final int status;



  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ErrorCode.unknownDefaultOpenApi,
  )


  final ErrorCode code;



  @JsonKey(
    
    name: r'detail',
    required: false,
    includeIfNull: false,
  )


  final String? detail;



  @JsonKey(
    
    name: r'traceId',
    required: false,
    includeIfNull: false,
  )


  final String? traceId;



      /// Alan yolu → hata kodları
  @JsonKey(
    
    name: r'errors',
    required: false,
    includeIfNull: false,
  )


  final Map<String, List<String>>? errors;



      /// Koda özgü ek bilgi (alternatives, attemptsLeft, retryAfterSeconds, brandCustomerId, planLimit, changedBy…)
  @JsonKey(
    
    name: r'meta',
    required: false,
    includeIfNull: false,
  )


  final Map<String, Object>? meta;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Problem &&
      other.type == type &&
      other.title == title &&
      other.status == status &&
      other.code == code &&
      other.detail == detail &&
      other.traceId == traceId &&
      other.errors == errors &&
      other.meta == meta;

    @override
    int get hashCode =>
        type.hashCode +
        title.hashCode +
        status.hashCode +
        code.hashCode +
        detail.hashCode +
        traceId.hashCode +
        errors.hashCode +
        meta.hashCode;

  factory Problem.fromJson(Map<String, dynamic> json) => _$ProblemFromJson(json);

  Map<String, dynamic> toJson() => _$ProblemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

