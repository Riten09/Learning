import { Injectable } from '@angular/core';
import { NgxSpinner, NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
busyRequestCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  busy(){
    this.busyRequestCount ++;
    this.spinnerService.show(undefined, {
      type: 'ball-climbing-dot',
      bdColor: 'rgba(255,255,255,0)',
      color:'#f7f7f7'
    })
  }

  idle(){
    this.busyRequestCount --;
    if(this.busyRequestCount <= 0){
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
