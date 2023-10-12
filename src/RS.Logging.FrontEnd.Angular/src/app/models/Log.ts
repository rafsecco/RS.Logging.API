export interface Log {
	id: number;
	createdAt: Date;
	logLevel: number;
	message: string;
	stackTrace: string;
}
