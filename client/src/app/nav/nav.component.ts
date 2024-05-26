import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { subscribeOn, take } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../_services/members.service';
import { UserParams } from '../_models/userParams';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  userParams: UserParams | undefined;
  user: User | undefined;

  constructor(public accountService: AccountService,private router: Router, private toastr: ToastrService,private memberSerive: MembersService){ }
  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: () => {
      
      this.accountService.currenUser$.pipe(take(1)).subscribe({
        next: user =>{
          if(user){
            this.memberSerive.setUserParams(new UserParams(user))
            this.router.navigateByUrl('/members')
          }
        }
      })
      
      }
      
      
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
