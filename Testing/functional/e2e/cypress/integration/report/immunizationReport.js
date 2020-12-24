const { AuthMethod } = require("../../support/constants")
let immunizationAPIPath=""
describe('Immunization History Report', () => {
    let sensitiveDocText = ' The file that you are downloading contains personal information. If you are on a public computer, please ensure that the file is deleted before you log off. ';
    
    before(() => {
        cy.readConfig().as("config").then(config => {
          config.webClient.modules.CovidLabResults = false
          config.webClient.modules.Comment = false
          config.webClient.modules.Encounter = false
          config.webClient.modules.Immunization = true
          config.webClient.modules.Laboratory = false
          config.webClient.modules.Medication = false
          config.webClient.modules.MedicationHistory = false
          config.webClient.modules.Note = false
          immunizationAPIPath=config.serviceEndpoints.Immunization
          cy.fixture("ImmunizationService/immunizationrefresh.json").then((ImmunizationResponse) => {
            cy.server();
            cy.route('GET', '/v1/api/configuration/', config);
            cy.route('GET', `${immunizationAPIPath}v1/api/Immunization/*`, ImmunizationResponse);
            cy.log("Configuration", config);
            cy.log("ImmunizationResponse", ImmunizationResponse)
          })
        })
        cy.login(Cypress.env('keycloak.username'), Cypress.env('keycloak.password'), AuthMethod.KeyCloak, "/reports");
    })

    it('Validate Immunization Loading', () => {      
        cy.get('[data-testid=reportType]')
            .should('be.enabled', 'be.visible')
            .select("Immunization")
    
            cy.get('[data-testid=timelineLoading]')
                .should('be.visible')
            cy.fixture("ImmunizationService/immunization.json").then((ImmunizationResponse) => {
              cy.server();
              cy.route('GET', `${immunizationAPIPath}v1/api/Immunization/*`, ImmunizationResponse);
              cy.log("ImmunizationResponse", ImmunizationResponse);
              cy.get('[data-testid=timelineLoading]')
                .should('be.visible')
            })
    })

    it('Validate Immunization History Report', () => { 
        cy.get('[data-testid=reportType]')
            .should('be.enabled', 'be.visible')
            .select("Immunization")

        cy.get('[data-testid=immunizationHistoryReportSample]')
            .should('be.visible')
        
        cy.get('[data-testid=immunizationDateTitle]')
        .should('be.visible')
        cy.get('[data-testid=immunizationProviderTitle]')
        .should('be.visible')
        cy.get('[data-testid=immunizationProductTitle]')
        .should('be.visible')
        cy.get('[data-testid=immunizationLotTitle]')
        .should('be.visible')

        cy.get('[data-testid=immunizationItemDate]')
            .last()
            .contains(/\d{4}-\d{2}-\d{2}/);
        cy.get('[data-testid=immunizationItemProduct]')
            .should('be.visible')
        cy.get('[data-testid=immunizationItemProviderClinic]')
            .should('be.visible')
        cy.get('[data-testid=immunizationItemLotNumber]')
            .should('be.visible')

        cy.get('[data-testid=exportRecordBtn]')
            .should('be.enabled', 'be.visible')
            .click();

        cy.get('[data-testid=genericMessageModal]')
            .should('be.visible');

        cy.get('[data-testid=genericMessageText]')
            .should('have.text', sensitiveDocText);

        cy.get('[data-testid=genericMessageSubmitBtn]')
            .click();
            
        cy.get('[data-testid=genericMessageModal]')
            .should('not.be.visible');
    })
}) 