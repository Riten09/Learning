import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  // members: Member[] = [] 
  members$: Observable<Member[]> | undefined; 
  constructor(private memberService: MembersService) { }


  ngOnInit(): void {
    // this.loadMember();
    this.members$ = this.memberService.getMembers();
  }

  // commenting below line as we are using Observable now 
  // loadMember() {
  //   this.memberService.getMembers().subscribe({
  //     next: members => this.members = members
  //   })
  // }

}
