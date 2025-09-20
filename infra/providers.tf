terraform {
  required_version = ">= 1.0"

  backend "azurerm" {
    resource_group_name  = "expo-resource-group"
    storage_account_name = "coproexpostorageaccount"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = "b214a9e0-1f17-4859-a558-850ffbf19a6f"
}