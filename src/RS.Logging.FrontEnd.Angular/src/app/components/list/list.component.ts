import { Component } from '@angular/core';
import { Log } from 'src/app/models/Log';
import { LoggingApiService } from 'src/app/shared/services/logging-api.service';

@Component({
	selector: 'app-list',
	templateUrl: './list.component.html',
	styleUrls: ['./list.component.scss']
})
export class ListComponent {

	log = {} as Log;
	public logs: Log[] = [];

	constructor(private logService: LoggingApiService) {
		this.LoadLogList();
	}

	public LoadLogList(): void {
		this.logService.getAll().subscribe({
			next: (_logs: Log[]) => this.logs = _logs,
			error: (error: any) => { console.log('Error loading LogList.', 'Error!'); }
		});
	}

}
