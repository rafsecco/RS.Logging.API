using Microsoft.Extensions.Logging;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Infra.Contexts;

namespace RS.Logstream.Infra.Seed;

public class DataSeeder(RSLoggingDbContext context)
{
	public void Seed()
	{
		if (context.Logs.Any()) return;

		// ── Logs gerais ───────────────────────────────────────────────────────
		context.Logs.AddRange(
			new Log(LogLevel.Information, "Aplicação iniciada com sucesso",
				pTenantId: "tenantA", pCorrelationId: "corr-seed-001"),
			new Log(LogLevel.Warning, "Cache miss — fallback para banco",
				pTenantId: "tenantA", pCorrelationId: "corr-seed-001"),
			new Log(LogLevel.Error, "Falha ao processar pagamento",
				"at PaymentService.Charge() line 42\nat OrderController.Post() line 17",
				pTenantId: "tenantA", pCorrelationId: "corr-seed-002"),
			new Log(LogLevel.Information, "Relatório gerado com sucesso",
				pTenantId: "tenantB")
		);
		context.SaveChanges();

		// ── Processo com sucesso ──────────────────────────────────────────────
		var processOk = new LogProcess(101, "Pedido #1001 — checkout",
			pTenantId: "tenantA", pCorrelationId: "corr-seed-001");
		context.LogProcess.Add(processOk);
		context.SaveChanges();

		context.LogProcessDetails.AddRange(
			new LogProcessDetail(processOk.Id, LogLevel.Information,
				"Etapa 1: carrinho validado", pCorrelationId: "corr-seed-001"),
			new LogProcessDetail(processOk.Id, LogLevel.Information,
				"Etapa 2: pagamento aprovado", pCorrelationId: "corr-seed-001"),
			new LogProcessDetail(processOk.Id, LogLevel.Information,
				"Etapa 3: estoque reservado", pCorrelationId: "corr-seed-001")
		);
		context.SaveChanges();

		// ── Processo com erro ─────────────────────────────────────────────────
		var processErr = new LogProcess(102, "Pedido #1002 — checkout",
			pTenantId: "tenantA", pCorrelationId: "corr-seed-002");
		context.LogProcess.Add(processErr);
		context.SaveChanges();

		context.LogProcessDetails.AddRange(
			new LogProcessDetail(processErr.Id, LogLevel.Information,
				"Etapa 1: carrinho validado", pCorrelationId: "corr-seed-002"),
			new LogProcessDetail(processErr.Id, LogLevel.Error,
				"Etapa 2: pagamento recusado — saldo insuficiente",
				"at PaymentService.Charge() line 42",
				pCorrelationId: "corr-seed-002")
		);
		context.SaveChanges();

		// ── ApiCallLog ────────────────────────────────────────────────────────
		context.ApiCallLogs.AddRange(
			new ApiCallLog("https://api.pagamentos.com/v1/charge", "POST", true,
				pRequestBody: """{"amount":199.90,"currency":"BRL"}""",
				pResponseStatusCode: 200,
				pResponseBody: """{"chargeId":"ch_abc123","status":"approved"}""",
				pDurationMs: 145,
				pTenantId: "tenantA", pCorrelationId: "corr-seed-001"),
			new ApiCallLog("https://api.pagamentos.com/v1/charge", "POST", false,
				pRequestBody: """{"amount":99.90,"currency":"BRL"}""",
				pResponseStatusCode: 402,
				pResponseBody: """{"error":"insufficient_funds"}""",
				pDurationMs: 210,
				pErrorMessage: "Payment declined",
				pTenantId: "tenantA", pCorrelationId: "corr-seed-002"),
			new ApiCallLog("https://api.estoque.com/v1/reservar", "POST", false,
				pRequestBody: """{"productId":42,"qty":1}""",
				pResponseStatusCode: null,
				pResponseBody: null,
				pDurationMs: 30000,
				pErrorMessage: "Connection refused: api.estoque.com:443",
				pTenantId: "tenantB")
		);
		context.SaveChanges();
	}
}
