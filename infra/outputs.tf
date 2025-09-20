output "web_app_name" {
  value = azurerm_windows_web_app.main.name
}

output "web_app_url" {
  value = azurerm_windows_web_app.main.default_hostname
}

output "key_vault_name" {
  value = azurerm_key_vault.main.name
}

output "key_vault_uri" {
  value = azurerm_key_vault.main.vault_uri
}

output "service_plan_name" {
  value = azurerm_service_plan.main.name
}