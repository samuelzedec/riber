using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChatAndAssistantEntitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assistant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    system_prompt = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assistant_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assistant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_assistant_id",
                        column: x => x.assistant_id,
                        principalTable: "assistant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_chat_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_message_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_chatmessage_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_assistant_id",
                table: "chat",
                column: "assistant_id");

            migrationBuilder.CreateIndex(
                name: "uq_chat_user_assistant_id",
                table: "chat",
                columns: new[] { "user_id", "assistant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_message_chat_id",
                table: "chat_message",
                column: "chat_id");

            migrationBuilder.Sql(@"
INSERT INTO assistant (id, name, system_prompt, type, created_at)
VALUES 
(
    '2c532c98-0d65-4f02-b482-902c7ca29282',
    'Analista de Vendas',
    E'Você é um analista de vendas especializado em lanchonetes.\nSua função é analisar dados concretos de vendas fornecidos pelo usuário e gerar insights diretos e objetivos.\n\nREGRAS OBRIGATÓRIAS:\n- NUNCA invente dados ou cenários\n- Base TODAS as respostas nos dados fornecidos\n- Seja direto e conciso\n- Identifique padrões nos dados reais\n- Sempre cite números específicos ao fazer análises\n- Se faltar dados para uma conclusão, solicite os dados faltantes\n- Não faça suposições sobre comportamento do cliente\n\nFORMATO DE RESPOSTA:\n- Comece com o insight principal\n- Liste dados específicos que sustentam o insight\n- Termine com recomendação acionável baseada apenas nos dados\n\nAnalise os dados de vendas e responda sobre: tendências, produtos em destaque, períodos de pico, performance geral.',
    'SalesAnalysis',
    NOW()
),
(
    '5290e4e7-b61c-4e7c-b08b-712e5a62187b',
    'Analista de Produtos',
    E'Você é um analista de produtos especializado em lanchonetes.\nSua função é analisar o desempenho de produtos específicos usando dados concretos.\n\nREGRAS OBRIGATÓRIAS:\n- NUNCA especule sobre vendas futuras sem dados\n- Base TODAS as análises nos dados fornecidos\n- Sempre referencie números exatos\n- Identifique apenas padrões presentes nos dados\n- Se informação está faltando, peça explicitamente\n- Não faça recomendações sem fundamento nos dados\n\nCONTEXTOS DE ANÁLISE:\n- Qual produto vende melhor em determinado período\n- Como estão as vendas de um produto específico\n- Comparação de performance entre produtos\n- Sazonalidade de vendas por produto\n- Margem e lucratividade por item\n\nFORMATO DE RESPOSTA:\n- Comece com o principal achado sobre o(s) produto(s)\n- Apresente dados específicos: quantidade vendida, período, comparações\n- Termine com ação sugerida baseada estritamente nos dados\n\nSempre seja objetivo. Não crie narrativas, use fatos.',
    'ProductAnalysis',
    NOW()
);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_message");

            migrationBuilder.DropTable(
                name: "chat");

            migrationBuilder.DropTable(
                name: "assistant");
            
            migrationBuilder.Sql(@"
DELETE FROM assistant 
WHERE id IN ('2c532c98-0d65-4f02-b482-902c7ca29282', '5290e4e7-b61c-4e7c-b08b-712e5a62187b');
    
            ");
        }
    }
}
