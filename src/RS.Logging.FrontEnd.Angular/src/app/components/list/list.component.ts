import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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

	page: number;
	pageSize: number;

	constructor(
		private logService: LoggingApiService,
		private router: Router,
		private activatedRouter: ActivatedRoute
	) {
		this.page = +this.activatedRouter.snapshot.params['page'];
		this.pageSize = +this.activatedRouter.snapshot.params['pageSize'];
		this.LoadLogList();
	}

	public LoadLogList(): void {
		this.logService.getAll(this.page, this.pageSize).subscribe({
			next: (_logs: Log[]) => this.logs = _logs,
			error: (error: any) => { console.log(`Error loading Logs.\n${error}`); }
		});
	}

	openEdit(id: number): void {
		this.router.navigate([`/detail/${id}`]);
	}

}
