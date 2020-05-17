import { ScIntegrationRefAppPage } from './app.po';

describe('sc-integration-ref-app App', () => {
  let page: ScIntegrationRefAppPage;

  beforeEach(() => {
    page = new ScIntegrationRefAppPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to app!');
  });
});
