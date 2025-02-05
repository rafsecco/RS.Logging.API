export enum LogLevel {
	Trace = 0,				//LogTrace 	Contêm as mensagens mais detalhadas. Essas mensagens podem conter dados confidenciais do aplicativo. Essas mensagens são desabilitadas por padrão e não devem ser habilitadas em um ambiente de produção.
	Debug = 1,				//LogDebug 	Para depuração e desenvolvimento. Use com cuidado em produção devido ao alto volume.
	Information = 2,	//LogInformation 	Rastreia o fluxo geral do aplicativo. Pode ter um valor de longo prazo.
	Warning = 3, 			//LogWarning 	Para eventos anormais ou inesperados. Geralmente, inclui erros ou condições que não fazem com que o aplicativo falhe.
	Error = 4,				//LogError 	Para erros e exceções que não podem ser manipulados. Essas mensagens indicam uma falha na operação ou na solicitação atual, não uma falha em todo o aplicativo.
	Critical = 5,			//LogCritical 	Para falhas que exigem atenção imediata. Exemplos: cenários de perda de dados, espaço em disco insuficiente.
	None = 6					//Especifica que uma categoria de log não deve gravar mensagens.
}
