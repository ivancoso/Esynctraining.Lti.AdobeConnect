function Set-Environment([Xml]$config) {
    
    $aspNodeCore = $config.SelectSingleNode("//aspNetCore")

    if (!$aspNodeCore)
    {
        throw "It is not asp core application"
    }

    $environmentVariablesNode = $config.CreateElement("environmentVariables");
    $environmentVariableNode = $config.CreateElement("environmentVariable");
    $environmentVariablesNode.AppendChild($environmentVariableNode)

    $nameAttrib = $environmentVariableNode.OwnerDocument.CreateAttribute("name")
    $nameAttrib.Value = "ASPNETCORE_ENVIRONMENT"

    $valueAttrib = $environmentVariableNode.OwnerDocument.CreateAttribute("value")
    $valueAttrib.Value = $environment

    $environmentVariableNode.Attributes.Append($nameAttrib)
    $environmentVariableNode.Attributes.Append($valueAttrib)

    $aspNodeCore.AppendChild($environmentVariablesNode)
}

