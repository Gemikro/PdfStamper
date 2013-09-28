<?xml version="1.0" ?>
<xsl:transform 

	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	version="1.0" 
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	
	
	xmlns:gmi="urn:gmi:nova:develop" 

	exclude-result-prefixes="xs">
	<xsl:output method="html" encoding="utf-8" indent="yes" />


	<xsl:template match="xs:schema">
	
////////////////////////////////////////////////////////////////////////////////////
// This file was created via tool. 
// Do not modify by hand, use tool to re-create it. 
////////////////////////////////////////////////////////////////////////////////////
	
using System;
using System.Collections;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;
using GMI.Core;

namespace <xsl:value-of select="@gmi:namespace" />{

	<xsl:if test="@gmi:extra_using">		
	using <xsl:value-of select="@gmi:extra_using" />;
	</xsl:if>	
	
	<xsl:call-template name="elements_handlers" />
}
	</xsl:template>
	
	<xsl:template name="elements_handlers">
	////////////////////////////////////////////////////////////////////////////////////
	/// <xsl:element name="summary"> ProcessXml handler class for request from 
	/// <xsl:value-of select="@targetNamespace" /> namespace.
	///
	/// Created via tool.</xsl:element>
	////////////////////////////////////////////////////////////////////////////////////

	[GMI_ProcessXmlHandler(Namespace="<xsl:value-of select="@targetNamespace" />" <xsl:choose><xsl:when test="@gmi:schema_file">, SchemaFile="<xsl:value-of select="@gmi:schema_file" />"</xsl:when></xsl:choose>)]
	public class ProcessXmlHandler{
		
			<xsl:for-each select="//xs:schema/xs:element[@gmi:lego]">
					
		<xsl:if test="@gmi:is_blox='true'">
		/// <xsl:element name="summary"> Returns info about parameters</xsl:element>
		public blox_info_response <xsl:value-of select="@name" />_Blox_GetInfo(GMI_Session session){
			
		    blox_info_response bi = new blox_info_response();
			bi.description = @"<xsl:value-of select="xs:annotation/xs:documentation" />";
			
		    bi.parameter = new blox_parameter[<xsl:value-of select="count(xs:complexType/xs:sequence/xs:element)"></xsl:value-of>];
	
				<xsl:for-each select="xs:complexType/xs:sequence/xs:element">
					<xsl:variable name="tmp_pos" select="position()-1"></xsl:variable>
			bi.parameter[<xsl:value-of select="$tmp_pos"></xsl:value-of>] = new blox_parameter();
			bi.parameter[<xsl:value-of select="$tmp_pos"></xsl:value-of>].name = "<xsl:value-of select="@name"></xsl:value-of>";
			bi.parameter[<xsl:value-of select="$tmp_pos"></xsl:value-of>].description = @"<xsl:value-of select="xs:annotation/xs:documentation"></xsl:value-of>";
			bi.parameter[<xsl:value-of select="$tmp_pos"></xsl:value-of>].type = <xsl:choose>
					<xsl:when test="@type='xs:dateTime'">blox_parameter_type.DateTime</xsl:when>
					<xsl:when test="@type='xs:string'">blox_parameter_type.String</xsl:when>
					<xsl:when test="@type='xs:int'">blox_parameter_type.Int</xsl:when>
					<xsl:when test="@type='xs:decimal'">blox_parameter_type.Decimal</xsl:when>
					<xsl:when test="@type='xs:boolean'">blox_parameter_type.Boolean</xsl:when>
					<xsl:otherwise>XXX_ERROR_XXX</xsl:otherwise>
					</xsl:choose>;
			<xsl:if test="@gmi:default_value">bi.parameter[<xsl:value-of select="$tmp_pos"></xsl:value-of>].default_value = <xsl:value-of select="@gmi:default_value" />;</xsl:if>
				</xsl:for-each>
	            
		    return bi;
		}

		</xsl:if>				
				
		/// <xsl:element name="summary"> ProcessXml handler for request <xsl:value-of select="@name" /></xsl:element>
		[GMI_ProcessXmlHandlerCode("<xsl:value-of select="@name" />", true<xsl:if test="@gmi:is_blox='true'">, BloxInfoMethod="<xsl:value-of select="@name" />_Blox_GetInfo"</xsl:if>)]
		public string <xsl:value-of select="@name" />(GMI_Session session, XmlDocument xml_doc, bool is_async){
			<xsl:value-of select="@name" /> parameters = (<xsl:value-of select="@name" />) GMI_Memento.DeserializeXml(xml_doc, typeof(<xsl:value-of select="@name" />));
			GMI_LegoObject lego = new <xsl:value-of select="@gmi:lego" />(session, parameters);
			if (is_async) {
				GMI_AsyncLegoManager alm = new GMI_AsyncLegoManager(session, lego, "<xsl:value-of select="@name" />");				
				return "<xsl:element name="cookie">" + alm.Run() + "</xsl:element>";
			} else {
				lego.Run();
				return GMI_Memento.SerializeXml(lego.results, true);
			}
		}		
			</xsl:for-each>
	}
	</xsl:template>

</xsl:transform>
