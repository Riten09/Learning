import { HttpClient, HttpHandler, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCahce = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) { 
    this.accountService.currenUser$.pipe(take(1)).subscribe({
      next: user =>{
        if(user){
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
  }

  getMembers(userParams: UserParams) {
    const response = this.memberCahce.get(Object.values(userParams).join('-'));

    if(response) return of(response);

    let params = this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users',params).pipe(
      map (response => {
        this.memberCahce.set(Object.values(userParams).join('-'), response);
        console.log(response);
        return response;
      })
    );
  }

  getUserParam(){
    console.log(this.userParams)
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }


  getMember(username: string){
    const member = [... this.memberCahce.values()]
    .reduce((arr, elem) => arr.concat(elem.result), [])
    .find((member: Member) => member.userName === username);

    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member){
    return this.http.put<Member>(this.baseUrl + 'users/', member).pipe(
      map(() =>{
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member}
      })
    )
  }

  setMainPhoto(photoId: number)
  {
    return this.http.put(this.baseUrl +'users/set-main-photo/'+ photoId,{});
  }

  deletePhoto(photoId: number)
  {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId,{});
  }

  private getPaginatedResult<T>(url: string,params: HttpParams) {
    const paginated : PaginatedResult<T> = new PaginatedResult<T>;
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginated.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginated.pagination = JSON.parse(pagination);
        }
        return paginated;
      })
    );
  }

  private getPaginationHeader(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    return params;
  }

}