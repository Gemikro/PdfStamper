<?xml version="1.0" ?>

<!--
*****************************************************************************************

xsd2callers

This class performs creates callers of ProcessXml from XSD schema.

History:
xx.xx.xxxx Vik; created.
16.07.2008 Vik; added parameter that overrides namespace.
28.07.2008 Vik; added optional attribute that specifies response class (gmi:response_element)
*****************************************************************************************
-->

<xsl:transform 

	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	version="1.0" 
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	
	
	xmlns:gmi="urn:gmi:nova:develop" 

	exclude-result-prefixes="xs">
	<xsl:output method="text" encoding="utf-8" indent="yes" />
	<xsl:param name="forced_namespace" />

	<xsl:template match="xs:schema">
	
////////////////////////////////////////////////////////////////////////////////////
// This file was created via tool. 
// Do not modify by hand, use tool to re-create it. 
////////////////////////////////////////////////////////////////////////////////////
	
using System;
using System.Collections;
using System.Xml;

namespace <xsl:choose><xsl:when test="$forced_namespace=''"><xsl:value-of select="@gmi:namespace" /></xsl:when><xsl:otherwise><xsl:value-of select="$forced_namespace" /></xsl:otherwise></xsl:choose> {

	using Gmi.NovaClient.Utils;	
	<xsl:call-template name="elements_handlers" />
}
	</xsl:template>

	<xsl:template name="elements_handlers">
		<xsl:if test="//xs:schema/xs:element[@gmi:lego]">
		
		<xsl:for-each select="//xs:schema/xs:element[@gmi:lego]">				
	/// <xsl:element name="summary"> ProcessXml request <xsl:value-of select="@name" /></xsl:element>
	public partial class <xsl:value-of select="@name" />{
		public <xsl:choose>
      <xsl:when test="@gmi:response_element"><xsl:value-of select="@gmi:response_element" /></xsl:when>
      <xsl:otherwise><xsl:value-of select="@name" />_response</xsl:otherwise>      
    </xsl:choose> ProcessXml(INovaService service) {
			var res_s = service.ProcessXml(this.SerializeXml(), <xsl:choose><xsl:when test="@gmi:is_async='true'">true</xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>);
			return res_s.DeserializeXml2&lt;<xsl:value-of select="@name" />_response&gt;();
		}
	}
		</xsl:for-each>
		</xsl:if>
	</xsl:template>
</xsl:transform>
