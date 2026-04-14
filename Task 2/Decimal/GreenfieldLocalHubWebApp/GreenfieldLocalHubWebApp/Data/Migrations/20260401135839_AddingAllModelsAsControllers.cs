using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenfieldLocalHubWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingAllModelsAsControllers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    addressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    postalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.addressId);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.categoriesId);
                });

            migrationBuilder.CreateTable(
                name: "loyaltyAccount",
                columns: table => new
                {
                    loyaltyAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pointsBalance = table.Column<int>(type: "int", nullable: false),
                    loyaltyTier = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyaltyAccount", x => x.loyaltyAccountId);
                });

            migrationBuilder.CreateTable(
                name: "producers",
                columns: table => new
                {
                    producersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    producerImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producers", x => x.producersId);
                });

            migrationBuilder.CreateTable(
                name: "shoppingCart",
                columns: table => new
                {
                    shoppingCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    shoppingCartCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    shoppingCartStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppingCart", x => x.shoppingCartId);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    ordersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    addressId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    totalAmount = table.Column<float>(type: "real", nullable: false),
                    delivery = table.Column<bool>(type: "bit", nullable: false),
                    collection = table.Column<bool>(type: "bit", nullable: false),
                    deliveryType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderCollectionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    orderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryCountry = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.ordersId);
                    table.ForeignKey(
                        name: "FK_orders_address_addressId",
                        column: x => x.addressId,
                        principalTable: "address",
                        principalColumn: "addressId");
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    productsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    producersId = table.Column<int>(type: "int", nullable: false),
                    categoriesId = table.Column<int>(type: "int", nullable: false),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stockQuantity = table.Column<int>(type: "int", nullable: false),
                    productPrice = table.Column<float>(type: "real", nullable: false),
                    productAvailability = table.Column<bool>(type: "bit", nullable: false),
                    productImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.productsId);
                    table.ForeignKey(
                        name: "FK_products_categories_categoriesId",
                        column: x => x.categoriesId,
                        principalTable: "categories",
                        principalColumn: "categoriesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_producers_producersId",
                        column: x => x.producersId,
                        principalTable: "producers",
                        principalColumn: "producersId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loyaltyTransaction",
                columns: table => new
                {
                    loyaltyTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loyaltyAccountId = table.Column<int>(type: "int", nullable: false),
                    ordersId = table.Column<int>(type: "int", nullable: true),
                    loyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyaltyTransaction", x => x.loyaltyTransactionId);
                    table.ForeignKey(
                        name: "FK_loyaltyTransaction_loyaltyAccount_loyaltyAccountId",
                        column: x => x.loyaltyAccountId,
                        principalTable: "loyaltyAccount",
                        principalColumn: "loyaltyAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_loyaltyTransaction_orders_ordersId",
                        column: x => x.ordersId,
                        principalTable: "orders",
                        principalColumn: "ordersId");
                });

            migrationBuilder.CreateTable(
                name: "orderProducts",
                columns: table => new
                {
                    orderProductsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ordersId = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderProducts", x => x.orderProductsId);
                    table.ForeignKey(
                        name: "FK_orderProducts_orders_ordersId",
                        column: x => x.ordersId,
                        principalTable: "orders",
                        principalColumn: "ordersId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orderProducts_products_productsId",
                        column: x => x.productsId,
                        principalTable: "products",
                        principalColumn: "productsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shoppingCartItems",
                columns: table => new
                {
                    shoppingCartItemsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shoppingCartId = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<float>(type: "real", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppingCartItems", x => x.shoppingCartItemsId);
                    table.ForeignKey(
                        name: "FK_shoppingCartItems_products_productsId",
                        column: x => x.productsId,
                        principalTable: "products",
                        principalColumn: "productsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shoppingCartItems_shoppingCart_shoppingCartId",
                        column: x => x.shoppingCartId,
                        principalTable: "shoppingCart",
                        principalColumn: "shoppingCartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loyaltyTransaction_loyaltyAccountId",
                table: "loyaltyTransaction",
                column: "loyaltyAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_loyaltyTransaction_ordersId",
                table: "loyaltyTransaction",
                column: "ordersId");

            migrationBuilder.CreateIndex(
                name: "IX_orderProducts_ordersId",
                table: "orderProducts",
                column: "ordersId");

            migrationBuilder.CreateIndex(
                name: "IX_orderProducts_productsId",
                table: "orderProducts",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_addressId",
                table: "orders",
                column: "addressId");

            migrationBuilder.CreateIndex(
                name: "IX_products_categoriesId",
                table: "products",
                column: "categoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_products_producersId",
                table: "products",
                column: "producersId");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCartItems_productsId",
                table: "shoppingCartItems",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCartItems_shoppingCartId",
                table: "shoppingCartItems",
                column: "shoppingCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyaltyTransaction");

            migrationBuilder.DropTable(
                name: "orderProducts");

            migrationBuilder.DropTable(
                name: "shoppingCartItems");

            migrationBuilder.DropTable(
                name: "loyaltyAccount");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "shoppingCart");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "producers");
        }
    }
}
