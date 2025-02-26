param containerAppPrincipalId string
param systemName string

resource webPubSubServiceOwnerRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-05-01-preview' existing = {
  name: '12cf5a90-567b-43ae-8102-96cf46c7d9b4'
}

module webPubSubServiceOwnerRoleAssignment 'role-assignment.bicep' = {
  name: 'ra-${systemName}-${webPubSubServiceOwnerRoleDefinition.name}'
  params: {
    principalId: containerAppPrincipalId
    roleDefinitionId: webPubSubServiceOwnerRoleDefinition.id
  }
}
