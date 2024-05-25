import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  // members: Member[] = [] 
  // members$: Observable<Member[]> | undefined; 
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display:'Male'}, {value: 'female', display:'Female'}]


  constructor(private memberService: MembersService) { 
    this.userParams = this.memberService.getUserParam();
  }


  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  // commenting below line as we are using Observable now 
  // loadMember() {
  //   this.memberService.getMembers().subscribe({
  //     next: members => this.members = members
  //   })
  // }

  loadMembers(){
    if(this.userParams){
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if(response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
          // console.log(response.result, response.pagination);
        }
      })
    }
  }

  resetFilter(){
      this.userParams = this.memberService.resetUserParams();
      this.loadMembers();
    
  }

  pageChanged(event: any){
    if(this.userParams && this.userParams?.pageNumber !== event.page)
    {       
      this.userParams.pageNumber = event.page; 
      this.loadMembers();      
      
    }      
      
  }

}
