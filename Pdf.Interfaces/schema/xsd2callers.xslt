<?xml version="1.0" ?>

<!--
*****************************************************************************************

xsd2callers

This class performs creates callers of ProcessXml from XSD schema.

History:
xx.xx.xxxx Vik; created.
16.07.2008 Vik; added parameter that overrides namespace.
*****************************************************************************************
-->

<xsl:transform 

	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	version="1.0" 
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	
	
	xmlns:gmi="urn:gmi:nova:develop" 

	exclude-result-prefixes="xs">
	<xsl:output method="html" encoding="utf-8" indent="yes" />
	<xsl:param name="forced_namespace" />

	<xsl:template match="xs:schema">
	
////////////////////////////////////////////////////////////////////////////////////
// This file was created via tool. 
// Do not modify by hand, use tool to re-create it. 
////////////////////////////////////////////////////////////////////////////////////
	
using System;
using System.Collections;
using System.Xml;
using GMI.Core;

namespace <xsl:choose><xsl:when test="$forced_namespace=''"><xsl:value-of select="@gmi:namespace" /></xsl:when><xsl:otherwise><xsl:value-of select="$forced_namespace" /></xsl:otherwise></xsl:choose> {
	<xsl:call-template name="elements_handlers" />
}
	</xsl:template>

	<xsl:template name="elements_handlers">
		<xsl:if test="//xs:schema/xs:element[@gmi:lego]">

	////////////////////////////////////////////////////////////////////////////////////
	/// <xsl:element name="summary"> ProcessXml caller class for request from 
	/// <xsl:value-of select="@targetNamespace" /> namespace.
	///
	/// Created via tool.</xsl:element>
	////////////////////////////////////////////////////////////////////////////////////

	public class ProcessXmlCaller {
		
			<xsl:for-each select="//xs:schema/xs:element[@gmi:lego]">
					
				
		/// <xsl:element name="summary"> ProcessXml caller for request <xsl:value-of select="@name" /></xsl:element>
		public static <xsl:value-of select="@name" />_response <xsl:value-of select="@name" />(<xsl:value-of select="@name" /> data) {
				string res_s = ServiceUtil.Singleton.Service.ProcessXml(GmiUtils.SerializeXml(data), <xsl:choose><xsl:when test="@gmi:is_async='true'">true</xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>);
				<xsl:value-of select="@name" />_response res = (<xsl:value-of select="@name" />_response) GmiUtils.DeserializeXml(res_s, typeof(<xsl:value-of select="@name" />_response));
				return res;
		}
			</xsl:for-each>
	}
		</xsl:if>
		
	</xsl:template>

</xsl:transform>
