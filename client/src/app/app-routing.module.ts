import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './_members/member-edit/member-edit.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';

const routes: Routes = [
  {path: '', component: HomeComponent}, //path for home page, if url is blank 
  {path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'members', component: MemberListComponent},
      {path: 'members/:username', component: MemberDetailComponent, resolve: {member: memberDetailedResolver}},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate:[preventUnsavedChangesGuard]},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent},
    ]
  },
  {path: "errors", component: TestErrorComponent},
  {path: "not-found", component: NotFoundComponent},
  {path: "server-error", component: ServerErrorComponent},
  {path: '**', component: NotFoundComponent}, // ** means if url mentioned will not match with above path then it will go to home component 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
