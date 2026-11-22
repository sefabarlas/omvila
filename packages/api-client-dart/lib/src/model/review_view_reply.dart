//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'review_view_reply.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewViewReply {
  /// Returns a new [ReviewViewReply] instance.
  ReviewViewReply({

    required  this.text,

    required  this.repliedUtc,

     this.repliedByName,
  });

  @JsonKey(
    
    name: r'text',
    required: true,
    includeIfNull: false,
  )


  final String text;



  @JsonKey(
    
    name: r'repliedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime repliedUtc;



  @JsonKey(
    
    name: r'repliedByName',
    required: false,
    includeIfNull: false,
  )


  final String? repliedByName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewViewReply &&
      other.text == text &&
      other.repliedUtc == repliedUtc &&
      other.repliedByName == repliedByName;

    @override
    int get hashCode =>
        text.hashCode +
        repliedUtc.hashCode +
        (repliedByName == null ? 0 : repliedByName.hashCode);

  factory ReviewViewReply.fromJson(Map<String, dynamic> json) => _$ReviewViewReplyFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewViewReplyToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

