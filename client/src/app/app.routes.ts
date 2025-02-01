import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { authGuard } from './_guards/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent }, // For the Home Component
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {
        path: 'members',
        component: MemberListComponent,
      }, // For the Member List Component
      { path: 'members/:id', component: MemberDetailComponent }, // For the Member Detail Component
      { path: 'lists', component: ListsComponent }, // For the Lists Component
      { path: 'messages', component: MessagesComponent }, // For the Messages Component
    ],
  },
  { path: '**', component: HomeComponent, pathMatch: 'full' }, // Wildcard, no matching elements will be routed back to the Home Component
];
