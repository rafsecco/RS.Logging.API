import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root'
})
export class LoggingApiService {

	public baseUrl = "https://localhost:7000";

	constructor(private http: HttpClient) { }

	public postLogging(data: any) {
		return this.http.post(`${this.baseUrl}/CreateLog/`, data);
	}
}
