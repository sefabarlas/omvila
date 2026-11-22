//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/role_matrix_rows_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'role_matrix.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class RoleMatrix {
  /// Returns a new [RoleMatrix] instance.
  RoleMatrix({

    required  this.rows,
  });

      /// Ekrandaki sırayla
  @JsonKey(
    
    name: r'rows',
    required: true,
    includeIfNull: false,
  )


  final List<RoleMatrixRowsInner> rows;





    @override
    bool operator ==(Object other) => identical(this, other) || other is RoleMatrix &&
      other.rows == rows;

    @override
    int get hashCode =>
        rows.hashCode;

  factory RoleMatrix.fromJson(Map<String, dynamic> json) => _$RoleMatrixFromJson(json);

  Map<String, dynamic> toJson() => _$RoleMatrixToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

