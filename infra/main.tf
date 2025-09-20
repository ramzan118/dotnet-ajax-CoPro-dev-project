# Create App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "copro-app-service-plan"
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Windows"
  sku_name            = var.app_service_plan_sku
}

# Create Windows Web App
resource "azurerm_windows_web_app" "main" {
  name                = "copro-web-app-${random_string.suffix.result}"
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true
  client_certificate_mode = "Required"

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on  = false
    managed_pipeline_mode = "Integrated"
    use_32_bit_worker = true
    minimum_tls_version = "1.2"
    vnet_route_all_enabled = false

    application_stack {
      dotnet_version = "v6.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
  }
}

# Create Key Vault
resource "azurerm_key_vault" "main" {
  name                        = "copro-vault-${random_string.suffix.result}"
  resource_group_name         = var.resource_group_name
  location                    = var.location
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  enable_rbac_authorization   = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Recover", "Backup", "Restore"
    ]
  }

  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }
}

# Store database secrets in Key Vault
resource "azurerm_key_vault_secret" "db_admin_user" {
  name         = "DbAdminUser"
  value        = var.sql_admin_username
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "db_admin_password" {
  name         = "DbAdminPassword"
  value        = var.sql_admin_password
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "db_port" {
  name         = "DbPort"
  value        = "1433"
  key_vault_id = azurerm_key_vault.main.id
}

# Random string for unique resource names
resource "random_string" "suffix" {
  length  = 8
  special = false
  upper   = false
}

# Data source for current Azure client config
data "azurerm_client_config" "current" {}

# Create SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = "copro-db-server-${random_string.suffix.result}"
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  
  # Explicitly set public network access
  public_network_access_enabled = true
  
  # Add minimum TLS version
  minimum_tls_version = "1.2"
  
  # Add identity for better security
  identity {
    type = "SystemAssigned"
  }
}

# Create Elastic Pool
resource "azurerm_mssql_elasticpool" "main" {
  name                = "copro-elastic-pool-${random_string.suffix.result}"
  resource_group_name = var.resource_group_name
  location            = var.location
  server_name         = azurerm_mssql_server.main.name
  license_type        = "LicenseIncluded"
  max_size_gb         = 4.8828125

  sku {
    name     = "BasicPool"
    tier     = "Basic"
    capacity = 50
  }

  per_database_settings {
    min_capacity = 0
    max_capacity = 5
  }
  
  # Add depends_on to ensure proper creation order
  depends_on = [azurerm_mssql_server.main]
}

# Create Database in Elastic Pool
resource "azurerm_mssql_database" "main" {
  name            = "copro-db-${random_string.suffix.result}"
  server_id       = azurerm_mssql_server.main.id
  elastic_pool_id = azurerm_mssql_elasticpool.main.id
  collation       = "SQL_Latin1_General_CP1_CI_AS"
  sku_name        = "ElasticPool"
  
  # Add depends_on to ensure proper creation order
  depends_on = [azurerm_mssql_elasticpool.main]
}

# Add additional Key Vault secrets for SQL
resource "azurerm_key_vault_secret" "db_host" {
  name         = "DbHost"
  value        = azurerm_mssql_server.main.fully_qualified_domain_name
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "db_name" {
  name         = "DbName"
  value        = azurerm_mssql_database.main.name
  key_vault_id = azurerm_key_vault.main.id
}