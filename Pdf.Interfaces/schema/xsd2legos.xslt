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
	
using System.Collections;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;
using GMI.Core;

namespace <xsl:value-of select="@gmi:namespace" />{
	<xsl:call-template name="elements_handlers" />
	<xsl:call-template name="elements_plugins" />
}
	</xsl:template>

	<xsl:template name="elements_handlers">
		<xsl:if test="//xs:schema/xs:element[@gmi:lego]">
			<xsl:for-each select="//xs:schema/xs:element[@gmi:lego]">
	
	////////////////////////////////////////////////////////////////////////////////////
	/// <xsl:element name="summary"> Lego class for request <xsl:value-of select="@name" />.
	/// Created via tool.</xsl:element>
	/// <xsl:element name="remarks"> <xsl:element name="history"> <xsl:element name="list"> <xsl:attribute name="type">bullet</xsl:attribute>
	/// <xsl:element name="item">xx.xx.xxxx yyyy; created</xsl:element>
	///	</xsl:element> </xsl:element> </xsl:element>
	////////////////////////////////////////////////////////////////////////////////////

	public class <xsl:value-of select="@gmi:lego" />: GMI_LegoObject{
	
		/// <xsl:element name="summary"> Input parameters in binary form. </xsl:element>
		///
		<xsl:value-of select="@name" /> input_parameters;
				
		/// <xsl:element name="summary"> constructor that accepts binary parameters </xsl:element>
		public <xsl:value-of select="@gmi:lego" />(GMI_Session session, <xsl:value-of select="@name" /> input_parameters) : base(session, input_parameters){
			this.input_parameters = input_parameters;
        	}
	
		/// <xsl:element name="summary"> main business logic </xsl:element>
		protected override void RunBl(){	
			// TODO perform all that is needed
			
			// store binary results into member results
			this.results = new <xsl:value-of select="@name" />_response();
		}
	}
			</xsl:for-each>
		</xsl:if>
	</xsl:template>


	<xsl:template name="elements_plugins">
		<xsl:for-each select="//xs:schema/xs:element[@gmi:plugin]">
			
			
	////////////////////////////////////////////////////////////////////////////////////
	/// <xsl:element name="summary"> Lego class for request <xsl:value-of select="@name" />.
	/// Created via tool.</xsl:element>
	////////////////////////////////////////////////////////////////////////////////////

	public class <xsl:value-of select="@gmi:plugin" />: GMI_LegoObject{
	
		/// <xsl:element name="summary"> Input parameters in binary form. </xsl:element>
		///
		<xsl:value-of select="@name" /> input_parameters;
		
		
		/// <xsl:element name="summary"> constructor that accepts binary parameters </xsl:element>
		public <xsl:value-of select="@gmi:plugin" />(GMI_Session session, <xsl:value-of select="@name" /> input_parameters) : base(session, input_parameters){
			this.input_parameters = input_parameters;
        	}
	
		/// <xsl:element name="summary"> main business logic </xsl:element>
		protected override void RunBl(){	
			// perform all that is needed

			// store binary results into member results
			this.results = new <xsl:value-of select="@name" />_response();
		}
	}
		</xsl:for-each>
		
	</xsl:template>


</xsl:transform>
