<?xml version="1.0" ?>

<!DOCTYPE xsl:stylesheet
  [
  <!ENTITY nbsp "&#xa0;">
  ]
>

<!-- xsd2html2.xslt

     converts XML schema definitions (which may even be embedded in WSDL or
     similar files) to HTML documentation

     authors: viktor jovanoski  <vik@g-gmi.si>
              jaka mocnik <jaka@xlab.si>

     changelog:

     20060126 jaka
     started the changelog, added support for element references, changed
     simple type layout, fixed support for in-lined types, fixed empty table
     cells - are now forced to shown by inserting a non-breakable space,
     added support for pattern restrictions, fixed layout of restrictions
     
     20060126 vik
     added support for more exotic restriction conditions like totalDigits, fractionDigits
	 
	 20060606 vik (yes, it's today is 666)
	added support for multiple languages (only target language is used, 
	others including default are ignored). target language is passed to script as target_lang.
	If used with MSXSL, use like this:
		msxsl.exe schema.xsd xsd2html_ml.xslt target_lang=en > schema.html
-->

<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	exclude-result-prefixes="xs">
	<xsl:output method="html" encoding="utf-8" indent="yes" />
	<xsl:param name="target_lang" />
	
	<!--*********************************************************************-->
	<!--** root of output                                                  **-->
	<!--*********************************************************************-->
	
<xsl:template match="xs:schema">
<html>
<head>
	<title>
		<xsl:value-of select="@targetNamespace" />
	</title>
	<style type='text/css'>
body{ color: #000000; font-family: Verdana,Tahoma, Arial, sans-serif; background-color: #ffffff; text-align: left; font-size: 10pt; }
td{	font-size: 10pt; border: 1px solid #dcdcdc; margin: 0px; text-align: left; padding: 0px 10px 0px 10px }
th{	border: 1px solid #dcdcdc; margin: 0px; text-align: left; padding: 0px 10px 0px 10px }
.noborder{	border: 0px; }
a{ background-repeat: no-repeat; text-decoration: none}
a, a:visited{ color: #0000aa; }
a:hover{ color: #aaaaaa; }
pre{ font-size: 9pt; }
h1{ font-family: Verdana, Tahoma, Arial, sans-serif; font-size: 18pt; font-weight: Bold; background-color: #003366; color: #ffffff; padding: 10px;}
h2{ font-family: Verdana, Tahoma, Arial, sans-serif; font-size: 15pt; font-weight: bold; color: #b4f40a; }
h3{ font-size: 12pt; font-weight: bold; color: #b4f40a; }
.small_text{ font-size: 8pt; color: #555555; }
	</style>
</head>
<body>
	<div class="small_text">
		Globus marine International d.o.o., Dunajska 51, SI-1000 Ljubljana, Slovenia <br />
					<b>Confidential</b>
	</div>
				
	<h1>
		Namespace: <code><xsl:value-of select="@targetNamespace" /></code>
	</h1>

	<p>
		<xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" />
	</p>

	<h2>Table of contents</h2>
<!-- jaKa: attributes -->
	Attributes:
	<xsl:call-template name="toc_attributes" />
<!-- /jaKa: attributes -->
	Elements:
	<xsl:call-template name="toc_elements" />
	Complex types:
	<xsl:call-template name="toc_ctypes" />
	Simple types:
	<xsl:call-template name="toc_stypes" />
	<hr />
<!-- jaKa: attributes -->
	<h2>Attributes</h2>
	<xsl:apply-templates select="xs:attribute" />
	<hr />
<!-- /jaKa: attributes -->
	<h2>Elements</h2>
	<xsl:apply-templates select="xs:element" />
	<hr />
	<h2>Complex types</h2>
	<xsl:apply-templates select="xs:complexType" />
	<hr />
	<h2>Simple types</h2>
	<xsl:apply-templates select="xs:simpleType" />
</body>
</html>
</xsl:template>

<!-- jaKa: attributes -->
	<!--*********************************************************************-->
	<xsl:template name="toc_attributes">
		<ul>
			<xsl:for-each select="//xs:schema/xs:attribute">
				<xsl:sort select="@name" />
				<li>
					<a href="#attribute-type-{@name}">
						<xsl:value-of select="@name" />
					</a>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>
<!-- /jaKa: attributes -->

	<!--*********************************************************************-->
	<xsl:template name="toc_elements">
		<ul>
			<xsl:for-each select="//xs:schema/xs:element">
				<xsl:sort select="@name" />
				<li>
					<a href="#element-type-{@name}">
						<xsl:value-of select="@name" />
					</a>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template name="toc_ctypes">
		<ul>
			<xsl:for-each select="//xs:schema/xs:complexType">
				<xsl:sort select="@name" />
				<li>
					<a href="#type-{@name}">
						<xsl:value-of select="@name" />
					</a>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template name="toc_stypes">
		<ul>
			<xsl:for-each select="//xs:schema/xs:simpleType">
				<xsl:sort select="@name" />
				<li>
					<a href="#type-{@name}">
						<xsl:value-of select="@name" />
					</a>
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template match="xs:simpleType">
		<xsl:choose>
			<!-- if standalone complex type -->
			<xsl:when test="@name">
				<h3 id="type-{@name}">
					Simple type: <xsl:value-of select="@name" />
				</h3>
			</xsl:when>
		</xsl:choose>

		<b>Description</b>: 
			<xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /> <br/>
		<b>Base type</b>: <code><xsl:value-of select="xs:restriction/@base" /></code><br/>
				<br/>
		<b>Restrictions:</b>
				<xsl:choose>
					<xsl:when test="xs:restriction">
						<xsl:apply-templates select="xs:restriction" />
					</xsl:when>
					<xsl:otherwise>-</xsl:otherwise>
				</xsl:choose>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template match="xs:complexType">
		<!-- if standalone complex type -->
		<xsl:if test="@name">
			<h3 id="type-{@name}">
				Complex type: <xsl:value-of select="@name" />
			</h3>
			<span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;
		</xsl:if>
		<xsl:choose>
			<xsl:when test="xs:all">
				<p>All</p>
			</xsl:when>
			<xsl:when test="xs:choice">
				<table>
					<tr>
						<th colspan="5">
							Choice
						</th>
						<tr>
							<th>Name</th>
							<th>Type</th>
							<th>Min</th>
							<th>Max</th>
							<th width="100%">Description</th>
						</tr>
					</tr>
					<xsl:apply-templates select="xs:choice" />
				</table>
			</xsl:when>
			<xsl:when test="xs:sequence">
				<table>
					<tr>
						<th colspan="5">Sequence</th>
					</tr>
					<tr>
						<th>Name</th>
						<th>Type/Ref</th>
						<th>Min</th>
						<th>Max</th>
						<th width="100%">Description</th>
					</tr>
					<xsl:apply-templates select="xs:sequence" />
				</table>
			</xsl:when>
			<xsl:when test="@mixed='true'">
				<p>Character content</p>
			</xsl:when>
			<xsl:otherwise>
				<p>Empty</p>
			</xsl:otherwise>
		</xsl:choose>
<!-- jaKa: support for attributes -->
    <xsl:if test="xs:attribute">
      <h4>Attributes</h4>
      <table>
      <tr><th>Name</th><th>Type/Ref</th><th>Description</th></tr>
      <xsl:apply-templates select="xs:attribute"/>
      </table>
    </xsl:if>
<!-- /jaKa: support for attributes -->
	</xsl:template>

<!-- jaKa: support for attributes -->
  <xsl:template match="xs:attribute">
    <xsl:choose>
			<xsl:when test="parent::xs:schema">
        <h3 id="attribute-type-{@name}"> <xsl:value-of select="@name"/> </h3>
        <table>
        <tr><th>Attribute</th><td> <xsl:value-of select="@name"/> </td></tr>
        <tr><th>Type</th><td> <xsl:value-of select="@type"/></td></tr>
        <tr><th>Description</th><td> <span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp; </td></tr>
        </table>
      </xsl:when>
			<xsl:when test="xs:simpleType">
				simple type
        <tr><td><xsl:value-of select="@name"/></td>
				<td><xsl:apply-templates select="xs:simpleType"/></td>
        <td><span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;</td></tr>
			</xsl:when>
      <xsl:when test="@name">
        <tr><td><xsl:value-of select="@name"/> </td>
        <td><xsl:value-of select="@type"/></td>
        <td><span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp; </td></tr>
      </xsl:when>
      <xsl:when test="@ref">
        <tr><td>
          <xsl:choose>
            <xsl:when test="@name"> <xsl:value-of select="@name"/> </xsl:when>
            <xsl:otherwise> <xsl:value-of select="@ref"/> </xsl:otherwise>
          </xsl:choose>
        </td>
        <td> <a href="#attribute-type-{@ref}"><xsl:value-of select="@ref"/></a> </td>
        <td> <span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp; </td></tr>
      </xsl:when>
    </xsl:choose>        
  </xsl:template>
<!-- /jaKa: support for attributes -->

	<!--*********************************************************************-->
	<xsl:template match="xs:element">
		<xsl:choose>
			<!-- standalone element -->
			<xsl:when test="parent::xs:schema">
				<h3 id="element-type-{@name}">
						Element: <xsl:value-of select="@name" />
					</h3>
				<span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;
				<xsl:choose>
					<xsl:when test="@type">
						<div id="type-{@name}">
							<table><tr><td>
								<xsl:choose>
									<xsl:when test="starts-with(@type, 'xs')">
										<xsl:value-of select="@type" />
									</xsl:when>
									<xsl:otherwise>
										<a href="#type-{@type}"><xsl:value-of select="@type" /></a>
									</xsl:otherwise>
								</xsl:choose>
							</td></tr></table>
						</div>
					</xsl:when>
					<xsl:when test="xs:complexType">
						<xsl:apply-templates select="xs:complexType" />
					</xsl:when>
					<xsl:when test="xs:simpleType">
						<xsl:apply-templates select="xs:simpleType" />
					</xsl:when>
					<xsl:otherwise>
						<p>Empty</p>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<!-- inside another type/element, but defined as separate type -->
			<xsl:when test="@type">
				<tr>
					<td>
						<xsl:value-of select="@name"/>
					</td>
					<td>
						<code>
							<xsl:choose>
								<xsl:when test="starts-with(@type, 'xs:')">
									<xsl:value-of select="@type" />
								</xsl:when>
								<xsl:otherwise>
									<a href="#type-{@type}">
										<xsl:value-of select="@type" />
									</a>
								</xsl:otherwise>
							</xsl:choose>
						</code>
					</td>
					<td>
						<xsl:choose>
							<xsl:when test="@minOccurs">
								<xsl:value-of select="@minOccurs" />
							</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:choose>
							<xsl:when test="@maxOccurs">
								<xsl:value-of select="@maxOccurs" />
							</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;
					</td>
				</tr>
			</xsl:when>
			<!-- nested structure -->
			<xsl:otherwise>
				<tr>
					<td>
						<xsl:choose>
							<xsl:when test="@name">
								<xsl:value-of select="@name" />
							</xsl:when>
							<xsl:when test="@ref">
								<xsl:value-of select="@ref" />
							</xsl:when>
							<xsl:otherwise>
								<b>Unable to deduce element name...</b>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:if test="@ref">
							<a href="#element-type-{@ref}"><xsl:value-of select="@ref"/></a>
						</xsl:if>
						<xsl:if test="xs:complexType">
							complex subtype
							<xsl:apply-templates select="xs:complexType" />
						</xsl:if>
						<xsl:if test="xs:simpleType">
							simple subtype
							<table>
								<xsl:apply-templates select="xs:simpleType" />
							</table>
						</xsl:if>
					</td>
					<td>
						<xsl:choose>
							<xsl:when test="@minOccurs">
								<xsl:value-of select="@minOccurs" />
							</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<xsl:choose>
							<xsl:when test="@maxOccurs">
								<xsl:value-of select="@maxOccurs" />
							</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</td>
					<td>
						<span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;
					</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template match="xs:group[@ref]" priority="1">
		<ul>
			<li>
				<xsl:text></xsl:text>
				<a href="#group-{@ref}">
					<xsl:value-of select="@ref" />
				</a>
				<xsl:text> group</xsl:text>
			</li>
		</ul>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template match="xs:restriction">
		<xsl:choose>
			<xsl:when test="xs:enumeration">
					<xsl:text> Enumeration</xsl:text>
				<ul>
					<xsl:apply-templates select="xs:enumeration" />
				</ul>
			</xsl:when>
			<xsl:when test="xs:pattern">
				<b>
					<xsl:text>Pattern:</xsl:text>
				</b>
				<xsl:value-of select="xs:pattern/@value"/>
			</xsl:when>
			<xsl:otherwise>
				<ul>
					<xsl:if test="xs:minInclusive">
						<li>
							Min inclusive: <xsl:value-of select="xs:minInclusive/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:maxInclusive">
						<li>
							Max inclusive: <xsl:value-of select="xs:maxInclusive/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:minExclusive">
						<li>
							Min exclusive: <xsl:value-of select="xs:minExclusive/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:maxExclusive">
						<li>
							Max exclusive: <xsl:value-of select="xs:maxExclusive/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:maxLength">
						<li>
							Max length: <xsl:value-of select="xs:maxLength/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:minLength">
						<li>
							Min length: <xsl:value-of select="xs:minLength/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:length">
						<li>
							Exact length: <xsl:value-of select="xs:length/@value" />
						</li>
					</xsl:if>

					<xsl:if test="xs:pattern">
						<li>
							Pattern: <xsl:value-of select="xs:pattern/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:whiteSpace">
						<li>
							White space: <xsl:value-of select="xs:whiteSpace/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:totalDigits">
						<li>
							Total digits: <xsl:value-of select="xs:totalDigits/@value" />
						</li>
					</xsl:if>
					<xsl:if test="xs:fractionDigits">
						<li>
							Fraction digits: <xsl:value-of select="xs:fractionDigits/@value" />
						</li>
					</xsl:if>
				</ul>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--*********************************************************************-->
	<xsl:template match="xs:enumeration">
		<li>
			<code>
				<b>
					<xsl:value-of select="@value" />
				</b>
			</code>:
			<xsl:if test="xs:annotation/xs:documentation[@xml:lang=$target_lang]">
				<span><xsl:value-of select="xs:annotation/xs:documentation[@xml:lang=$target_lang]" /></span>&nbsp;
			</xsl:if>
		</li>
	</xsl:template>

</xsl:transform>
