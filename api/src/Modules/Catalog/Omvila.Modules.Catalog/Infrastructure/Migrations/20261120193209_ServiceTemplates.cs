using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "service_templates",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    industry_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    category_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    default_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_templates", x => x.id);
                    table.CheckConstraint("ck_service_templates_duration", "default_duration_minutes BETWEEN 5 AND 720");
                });

            migrationBuilder.CreateIndex(
                name: "ix_service_templates_industry_code_name",
                schema: "catalog",
                table: "service_templates",
                columns: new[] { "industry_code", "name" },
                unique: true);

            // Platform verisi: markaya ait değil, RLS yok; bütün markalar okur. İçerik taslağı ve kaynağı:
            // docs/urun/hazir-hizmet-listeleri.md (acik-sorular.md T-31). Kimlikler sabittir (içerik güncellemesi aynı kimlikle).
            migrationBuilder.Sql("""
                INSERT INTO catalog.service_templates (id, industry_code, category_name, name, default_duration_minutes, sort_order, is_active) VALUES
                ('7c80ab70-467c-5c30-ba14-0503e3e6acf8', 'hair_salon', 'Kesim', 'Kadın saç kesimi', 45, 1, true),
                ('871635f1-f97f-5286-a5d1-fb46fe6da181', 'hair_salon', 'Kesim', 'Çocuk saç kesimi', 30, 2, true),
                ('ad4a798e-367b-54a1-b6d6-1460f3fae469', 'hair_salon', 'Kesim', 'Kahkül kesimi', 15, 3, true),
                ('2e63cd6d-af88-5865-a7a9-83d0696a9feb', 'hair_salon', 'Fön ve şekillendirme', 'Fön', 30, 4, true),
                ('44f95b07-5081-5614-b370-94aa51c26adc', 'hair_salon', 'Fön ve şekillendirme', 'Maşa', 45, 5, true),
                ('3eaf01e1-aacd-5548-b28b-5a7475f93715', 'hair_salon', 'Fön ve şekillendirme', 'Topuz', 60, 6, true),
                ('0c7d0aed-38c4-5b56-86d8-9029ad05b273', 'hair_salon', 'Fön ve şekillendirme', 'Gelin saçı', 120, 7, true),
                ('2cda5b81-bb82-5d0c-88a9-810dbf3a35c5', 'hair_salon', 'Renk', 'Dip boya', 90, 8, true),
                ('2bffb027-1ade-5286-8901-9e4ebfa40ed0', 'hair_salon', 'Renk', 'Tüm boya', 120, 9, true),
                ('3a69c5b7-35af-5671-8460-7cbd6beaad84', 'hair_salon', 'Renk', 'Röfle', 150, 10, true),
                ('1c6e6c12-d09f-5c80-8ce4-438c45a39aab', 'hair_salon', 'Renk', 'Balyaj', 180, 11, true),
                ('ff8710b0-987a-5c56-ade0-a9183cc8de8f', 'hair_salon', 'Renk', 'Ombre', 180, 12, true),
                ('c257dce6-900f-56cb-b286-214c4452478d', 'hair_salon', 'Renk', 'Renk açma', 150, 13, true),
                ('3d612fc6-8425-5267-a9b7-3ecf6f4e3102', 'hair_salon', 'Bakım', 'Keratin bakımı', 120, 14, true),
                ('b542f398-c37e-5104-9aae-efd85df26ca3', 'hair_salon', 'Bakım', 'Saç bakım maskesi', 30, 15, true),
                ('d6280968-ae19-5584-93dc-ff7735c72195', 'hair_salon', 'Bakım', 'Brezilya fönü', 150, 16, true),
                ('d738fb55-44aa-5e5a-a608-7cce90b9b877', 'hair_salon', 'Bakım', 'Perma', 120, 17, true),
                ('b38ff858-d316-5021-95ba-429032ad59e3', 'hair_salon', 'Bakım', 'Saç yıkama', 15, 18, true),
                ('23536793-234f-574c-82ee-d16506a116f2', 'barber', 'Kesim', 'Saç kesimi', 30, 1, true),
                ('4e31321e-a88b-5d40-8dff-5b242254c520', 'barber', 'Kesim', 'Çocuk saç kesimi', 20, 2, true),
                ('e0781299-b1f1-5e92-b2d6-055354270f47', 'barber', 'Kesim', 'Saç ve sakal', 45, 3, true),
                ('d661f0a8-b5ef-532e-be76-f91437925b58', 'barber', 'Sakal', 'Sakal tıraşı', 20, 4, true),
                ('d407b242-99a7-5dcc-b363-b3eca9adf88c', 'barber', 'Sakal', 'Sakal şekillendirme', 20, 5, true),
                ('d465db2d-0ac3-59d2-ba21-7b845c313bc8', 'barber', 'Sakal', 'Ustura tıraşı', 30, 6, true),
                ('88149414-ac97-5f07-b5d0-e7e6de1c5b86', 'barber', 'Bakım', 'Yüz maskesi', 20, 7, true),
                ('9321ab6f-3f64-5bcb-90c7-63afe6d53c95', 'barber', 'Bakım', 'Cilt temizliği', 30, 8, true),
                ('ae8c5ef2-58f6-5790-8ac0-cf44315cbff6', 'barber', 'Bakım', 'Kulak ve burun ağdası', 10, 9, true),
                ('703aa7f1-cda5-54b0-afff-03770aa3ebdd', 'barber', 'Renk', 'Saç boyama', 45, 10, true),
                ('acf8aac2-a7fe-5306-9698-a2dc4b38576c', 'barber', 'Renk', 'Sakal boyama', 20, 11, true),
                ('80e2d076-b167-5521-a86f-e284826c014d', 'barber', 'Kesim', 'Damat tıraşı', 90, 12, true),
                ('585dffad-c39e-5130-8ee3-2befe51ae9ff', 'beauty', 'Cilt bakımı', 'Klasik cilt bakımı', 60, 1, true),
                ('0452ca0c-f845-5803-9bf9-f54d1f5c1978', 'beauty', 'Cilt bakımı', 'Derin cilt temizliği', 75, 2, true),
                ('f922c71e-2914-5eb8-b95f-8d2cab7f7d62', 'beauty', 'Cilt bakımı', 'Hydrafacial', 60, 3, true),
                ('65199a12-74e2-5eed-b249-5efc8b237c6d', 'beauty', 'Ağda', 'Tüm bacak ağda', 45, 4, true),
                ('51eb9713-f247-597e-83e3-18dcfc037d36', 'beauty', 'Ağda', 'Yarım bacak ağda', 30, 5, true),
                ('7123682d-2ad5-57d8-bad5-1260c928ede0', 'beauty', 'Ağda', 'Koltuk altı ağda', 15, 6, true),
                ('e6a5c1a5-3084-5d11-99d1-4e847b6b1d18', 'beauty', 'Ağda', 'Yüz ağda', 15, 7, true),
                ('59357feb-1727-5b80-a149-6090b878f82d', 'beauty', 'Kaş ve kirpik', 'Kaş tasarımı', 30, 8, true),
                ('cc3e8699-5019-55e3-8990-987fbf16dd66', 'beauty', 'Kaş ve kirpik', 'Kaş laminasyonu', 45, 9, true),
                ('8caaf044-f9ee-5875-9c27-4814fb7fd85d', 'beauty', 'Kaş ve kirpik', 'Kirpik lifting', 60, 10, true),
                ('a27f2f4f-46a0-5902-816a-f86e28cf1686', 'beauty', 'Kaş ve kirpik', 'İpek kirpik', 120, 11, true),
                ('f7e303dd-2fce-587d-85c3-5966bf3ce1b7', 'beauty', 'Makyaj', 'Günlük makyaj', 45, 12, true),
                ('000d8ed6-fd39-5ab9-b636-05e7bd764f0a', 'beauty', 'Makyaj', 'Gece makyajı', 60, 13, true),
                ('77dba346-2044-5be4-b709-a7b9f4c4b472', 'beauty', 'Makyaj', 'Gelin makyajı', 120, 14, true),
                ('c1e7dc60-edb8-5c54-bc24-9b43f5bcead2', 'spa', 'Masaj', 'Klasik masaj', 60, 1, true),
                ('46318c49-2a65-56b4-a5e3-7acb4eb7f1e7', 'spa', 'Masaj', 'Aromaterapi masajı', 60, 2, true),
                ('e42926a7-cbfa-54cf-91c7-581cc0919b6f', 'spa', 'Masaj', 'Derin doku masajı', 60, 3, true),
                ('0b307c77-19b6-5bc1-a6c7-b67f4dc723b2', 'spa', 'Masaj', 'Sıcak taş masajı', 75, 4, true),
                ('fc3f7990-eb49-5e0f-a12b-2e710d3fe1db', 'spa', 'Masaj', 'Refleksoloji', 45, 5, true),
                ('e72ddb73-01b7-562e-9f9e-dbbaf1af9f8a', 'spa', 'Masaj', 'Sırt ve boyun masajı', 30, 6, true),
                ('eaa6a0df-1d3f-520a-87cc-d277216abac8', 'spa', 'Hamam', 'Kese köpük', 45, 7, true),
                ('36b1043a-2430-538c-985e-743dcce96425', 'spa', 'Hamam', 'Hamam ve masaj paketi', 105, 8, true),
                ('46a9c890-e68b-5ed8-b5a6-172ea4d1bac6', 'spa', 'Bakım', 'Vücut peelingi', 45, 9, true),
                ('7b6f2f3d-6617-55df-a154-c37a9786c89b', 'spa', 'Bakım', 'Vücut maskesi', 60, 10, true),
                ('80980fd5-86f4-563e-a1dd-242a208b45fc', 'nail', 'Manikür', 'Manikür', 45, 1, true),
                ('58ef19c6-3319-5b54-a018-b1a7653e73f2', 'nail', 'Manikür', 'Kalıcı oje', 60, 2, true),
                ('f9210d6f-0ce1-5cc1-bc0c-8fa92ce21271', 'nail', 'Manikür', 'Kalıcı oje çıkarma', 20, 3, true),
                ('b8e95e94-5a11-54ac-af48-e08cc96f19f0', 'nail', 'Pedikür', 'Pedikür', 60, 4, true),
                ('a531e849-0211-5f4b-8546-4f759a0a8a40', 'nail', 'Pedikür', 'Kalıcı oje pedikür', 75, 5, true),
                ('2d87ec32-f900-5e74-bfa9-478e80a2779b', 'nail', 'Protez tırnak', 'Jel tırnak', 90, 6, true),
                ('b2dc2165-c095-546f-a713-97d8f68b28c2', 'nail', 'Protez tırnak', 'Akrilik tırnak', 120, 7, true),
                ('411dff01-3a22-5a2b-8a4e-53cf20d72ca2', 'nail', 'Protez tırnak', 'Protez tırnak dolgu', 75, 8, true),
                ('9eeb6f50-719b-5ac3-ab67-0f69e592aee2', 'nail', 'Tasarım', 'Nail art', 30, 9, true),
                ('d092aed4-3dd5-50de-930b-440e432b9161', 'nail', 'Tasarım', 'Fransız manikür', 60, 10, true),
                ('0a864fdd-1908-53d9-bb9f-23b9a19c248d', 'dietitian', 'Görüşme', 'İlk görüşme', 60, 1, true),
                ('7fc05056-0714-5dfb-b4e2-76b3476a3bf9', 'dietitian', 'Görüşme', 'Kontrol görüşmesi', 30, 2, true),
                ('7cd7dfe5-2dbb-5798-9acc-886c631b59f9', 'dietitian', 'Görüşme', 'Online görüşme', 30, 3, true),
                ('f1a0b65d-2caa-553e-85a2-136ed415ad27', 'dietitian', 'Ölçüm', 'Vücut analizi', 15, 4, true),
                ('2fb937a8-e26f-52a2-9fec-d1ce88051421', 'dietitian', 'Program', 'Sporcu beslenmesi görüşmesi', 45, 5, true),
                ('07a64d1a-30b3-5fe4-8afe-378a79582cd3', 'dietitian', 'Program', 'Çocuk beslenmesi görüşmesi', 45, 6, true),
                ('32f59e33-1859-5f29-b90d-9e2adea6e6cc', 'other', 'Hizmetler', 'Danışmanlık', 30, 1, true),
                ('2b1ae024-8f18-5c20-8308-9b9776f2fc44', 'other', 'Hizmetler', 'Standart seans', 60, 2, true),
                ('b1d1ec33-b17a-526e-9c79-1ed2aa21318a', 'other', 'Hizmetler', 'Uzun seans', 90, 3, true),
                ('de2b0807-cdc7-5baf-811d-cfd4aff84b70', 'other', 'Hizmetler', 'Kısa seans', 15, 4, true);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_templates",
                schema: "catalog");
        }
    }
}
