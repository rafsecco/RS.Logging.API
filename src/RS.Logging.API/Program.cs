using Microsoft.AspNetCore.Mvc;
using RS.Logging.API.Configurations;
using RS.Logging.API.ViewModels;
using RS.Logging.Domain.Log;
using RS.Logging.Infra.Contexts;

var builder = WebApplication.CreateBuilder(args);

#region ConfigureService
builder.Services.AddApiConfigureServices(builder.Configuration);
builder.Services.AddSwaggerConfigureServices();
#endregion

var app = builder.Build();

#region Configure
app.UseApiConfigure();
app.UseSwaggerConfigure();
#endregion

#region Map Get
//app.MapGet("/Search/", ([FromServices] LogsDbContext dbContext, SearchLogsViewModel model) =>
//{
//	IQueryable<Log> query = dbContext.Logs.AsNoTracking();

//	if (model.DateTimeFim == null) { query = query.Where(p => p.CreatedAt >= model.DateTimeIni); }
//	else { query = query.Where(p => p.CreatedAt >= model.DateTimeIni && p.CreatedAt <= model.DateTimeFim); }

//	if (model.IdProcess != null) { query = query.Where(p => p.IdProcess == model.IdProcess); }
//	if (model.LogLevel != null) { query = query.Where(p => p.LogLevel == model.LogLevel); }
//	if (!string.IsNullOrEmpty(model.Message)) { query = query.Where(s => s.Message.Contains(model.Message)); }

//	query = query.OrderBy(o => o.CreatedAt);

//	#region Pagging
//	var totalResults = query.Count();
//	query = query.Skip(model.PageSize * (model.PageIndex - 1)).Take(model.PageSize);

//	var result = new PagedModel<Log>(
//		totalResults,
//		model.PageIndex,
//		model.PageSize,
//		query.ToList<Log>());
//	#endregion

//	return result;
//});
#endregion

#region Map Post
app.MapPost("/CreateLog/", ([FromServices] RSLoggingDbContext dbContext, [FromBody] LogsViewModel pModel) =>
{
	var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace);
	dbContext.Logs.Add(log);
	int result = dbContext.SaveChanges();
	return result;
});
#endregion

app.Run();
