import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Log } from 'src/app/models/Log';
import { LoggingApiService } from 'src/app/shared/services/logging-api.service';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})
export class DetailComponent {

	logId: number;
	public log = {} as Log;

	constructor(
		private logService: LoggingApiService,
		private activatedRouter: ActivatedRoute,
	) {
		this.logId = +this.activatedRouter.snapshot.params['id'];
		this.LoadLog();
	}

	public LoadLog(): void {
		if (this.logId !== null && this.logId !== 0) {
			this.logService.getById(this.logId).subscribe({
				next: (_log: Log) => this.log = _log,
				error: (error: any) => { console.log(`Error loading Log.\n${error}`); }
			});
		}
		else {
			console.log(`Error loading Log.`);
		}
	}

}
