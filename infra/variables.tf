variable "resource_group_name" {
  description = "Name of the resource group"
  default     = "expo-resource-group"
}

variable "location" {
  description = "Azure region"
  default     = "Central US"  # Changed from East US to Central US
}

variable "sql_admin_username" {
  description = "SQL Server administrator username"
  default     = "copro"
}

variable "sql_admin_password" {
  description = "SQL Server administrator password"
  sensitive   = true
}

variable "app_service_plan_sku" {
  description = "App Service Plan SKU"
  default     = "S1"  # Changed from B1 to S1
}