import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Log } from 'src/app/models/Log';
import { Observable } from 'rxjs';

@Injectable({
	providedIn: 'root'
})
export class LoggingApiService {

	public baseUrl = "https://localhost:7000";

	constructor(private http: HttpClient) { }

	public postLogging(data: any) {
		return this.http.post(`${this.baseUrl}/CreateLog/`, data);
	}

	public getAll(): Observable<Log[]> {
		return this.http.get<Log[]>(`${this.baseUrl}/GetAll/`);
	}

}
