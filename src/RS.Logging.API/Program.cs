using Microsoft.EntityFrameworkCore;
using RS.Core.Entities;
using RS.Logging.API.Configurations;
using RS.Logging.API.Database;
using RS.Logging.API.Domain;
using RS.Logging.API.ViewModels;

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

app.MapPost("/Search/", (LogsDbContext dbContext, SearchLogsViewModel model) =>
{
	IQueryable<Log> query = dbContext.Logs.AsNoTracking();

	if (model.DateTimeFim == null) { query = query.Where(p => p.DateCreated >= model.DateTimeIni); }
	else { query = query.Where(p => p.DateCreated >= model.DateTimeIni && p.DateCreated <= model.DateTimeFim); }

	if (model.IdProcess != null) { query = query.Where(p => p.IdProcess == model.IdProcess); }
	if (model.LogLevel != null) { query = query.Where(p => p.LogLevel == model.LogLevel); }
	if (!string.IsNullOrEmpty(model.Message)) { query = query.Where(s => s.Message.Contains(model.Message)); }

	query = query.OrderBy(o => o.DateCreated);

	#region Pagging
	var totalResults = query.Count();
	query = query.Skip(model.PageSize * (model.PageIndex - 1)).Take(model.PageSize);

	var result = new PagedModel<Log>(
		totalResults,
		model.PageIndex,
		model.PageSize,
		query.ToList<Log>());
	#endregion

	return result;
});

app.MapPost("/CreateLog/", (LogsDbContext dbContext, List<LogsViewModel> model) =>
{
	var list = new List<Log>();

	model.ForEach(log =>
		list.Add(new Log(log.IdProcess, log.LogLevel, log.Message, log.Info, log.DateCreated)));

	dbContext.Logs.AddRange(list);
	int result = dbContext.SaveChanges();

	return result;
});

app.Run();

