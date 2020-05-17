import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { ScAccountInformationModule } from '@speak/ng-bcl/account-information';
import { ScActionBarModule } from '@speak/ng-bcl/action-bar';
import { ScApplicationHeaderModule } from '@speak/ng-bcl/application-header';
import { ScButtonModule } from '@speak/ng-bcl/button';
import { ScGlobalHeaderModule } from '@speak/ng-bcl/global-header';
import { ScGlobalLogoModule } from '@speak/ng-bcl/global-logo';
import { ScIconModule } from '@speak/ng-bcl/icon';
import { ScPageModule } from '@speak/ng-bcl/page';
import { ScTabsModule } from '@speak/ng-bcl/tabs';
import { ScDropdownModule } from '@speak/ng-bcl/dropdown';
import { ScTableModule } from '@speak/ng-bcl/table'; 
import { ScMenuModule } from '@speak/ng-bcl/menu';
import { CONTEXT, DICTIONARY } from '@speak/ng-bcl';
import { ScActionControlModule } from '@speak/ng-bcl/action-control';
import { NgScModule } from '@speak/ng-sc';
import { SciAntiCSRFModule } from '@speak/ng-sc/anti-csrf';

import { ScDialogModule } from '@speak/ng-bcl/dialog';

import { SciLogoutService } from '@speak/ng-sc/logout';
import { AppComponent } from './app.component';
import { StartPageComponent } from './start-page/start-page.component';
import { ServiceSamplePageComponent } from './service-sample-page/service-sample-page.component';
import { SampleService } from './service-sample-page/sample.service';
import { HealthcheckService } from './healthcheck.service';
import { ScProgressIndicatorPanelModule } from "@speak/ng-bcl/progress-indicator-panel";

@NgModule({
  declarations: [
    AppComponent,
    StartPageComponent,
    ServiceSamplePageComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    HttpClientModule,
    RouterModule.forRoot([
      { path: '', component: StartPageComponent, pathMatch: 'full' },
      { path: 'servicesample', component: ServiceSamplePageComponent }
    ]),
    ScAccountInformationModule,
    ScActionBarModule,
    ScApplicationHeaderModule,
    ScButtonModule,
    ScGlobalHeaderModule,
    ScGlobalLogoModule,
    ScIconModule,
    ScPageModule,
    ScMenuModule,
    ScTabsModule,
    ScTableModule,
    ScDropdownModule,
    ScDialogModule,
    ScActionControlModule,
    ScProgressIndicatorPanelModule,
    SciAntiCSRFModule,
    NgScModule.forRoot({
      authItemId: '1023A91F-E7C0-410C-BE84-472204C71FD7',
      contextToken: CONTEXT,
      dictionaryToken: DICTIONARY
    })
  ],
  providers: [
    SampleService, SciLogoutService, HealthcheckService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
