<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:import href="codegen.xslt"/>

    <xsl:template name="codegen.process.fullproject">
        <folder name="Intellij Idea 10">
            <folder name="InvokerDemoApp">
                <xsl:variable name="intellij_idea_files_path" select="'invokerapps/android/idea/InvokerDemoApp/'"/>

                <xsl:call-template name="codegen.project.intellij_idea">
                    <xsl:with-param name="app_files_path" select="$intellij_idea_files_path"/>
                </xsl:call-template>

                <xsl:call-template name="codegen.project.res">
                    <xsl:with-param name="res_files_path" select="concat($intellij_idea_files_path, 'res/')"/>
                </xsl:call-template>

                <folder name="src">
                    <xsl:call-template name="codegen.project.src">
                        <xsl:with-param name="src_files_path" select="concat($intellij_idea_files_path, 'src/')"/>
                    </xsl:call-template>

                    <xsl:for-each select="/namespaces">
                        <xsl:call-template name="codegen.process.namespace"/>
                    </xsl:for-each>
                </folder>
            </folder>
        </folder>
        <folder name="Eclipse 3.6">
            <folder name="InvokerDemoApp">
                <xsl:variable name="eclipse_files_path" select="'invokerapps/android/eclipse/InvokerDemoApp/'"/>

                <xsl:call-template name="codegen.project.eclipse">
                    <xsl:with-param name="app_files_path" select="$eclipse_files_path"/>
                </xsl:call-template>

                <xsl:call-template name="codegen.project.res">
                    <xsl:with-param name="res_files_path" select="concat($eclipse_files_path, 'res/')"/>
                </xsl:call-template>

                <folder name="src">
                    <xsl:call-template name="codegen.project.src">
                        <xsl:with-param name="src_files_path" select="concat($eclipse_files_path, 'src/')"/>
                    </xsl:call-template>

                    <xsl:for-each select="/namespaces">
                        <xsl:call-template name="codegen.process.namespace"/>
                    </xsl:for-each>
                </folder>
            </folder>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.process.namespace">
        <xsl:for-each select="namespace">
            <folder name="{@name}">
                <xsl:call-template name="codegen.process.namespace"/>
                <xsl:for-each select="service">
                    <xsl:call-template name="codegen.service"/>
                </xsl:for-each>

                <xsl:for-each select="datatype">
                    <xsl:call-template name="codegen.datatype"/>
                </xsl:for-each>

                <xsl:for-each select="enum">
                    <xsl:call-template name="codegen.enum"/>
                </xsl:for-each>
            </folder>
        </xsl:for-each>
    </xsl:template>

    <xsl:template name="codegen.datatype">
        <file name="{concat(@name, '.java')}">
            <xsl:call-template name="codegen.description">
                <xsl:with-param name="file-name" select="concat(@name,'.java')"/>
            </xsl:call-template>
            <xsl:text>package </xsl:text><xsl:value-of select="@typeNamespace"/><xsl:text>;

public class </xsl:text><xsl:value-of select="@name"/><xsl:value-of select="@genericParameters"/><xsl:text> {
    </xsl:text>
            <xsl:for-each select="field">
                <xsl:text>
    public </xsl:text>
                <xsl:value-of select="@javatype"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="@name"/><xsl:text>;</xsl:text>
            </xsl:for-each>
            <xsl:text>
}</xsl:text>
        </file>
    </xsl:template>

    <xsl:template name="codegen.enum">
        <file name="{concat(@name, '.java')}">
            <xsl:call-template name="codegen.description">
                <xsl:with-param name="file-name" select="concat(@name,'.java')"/>
            </xsl:call-template>
            <xsl:text>package </xsl:text><xsl:value-of select="@typeNamespace"/><xsl:text>;

public </xsl:text><xsl:value-of select="name()"/><xsl:text> </xsl:text><xsl:value-of select="@name"/><xsl:text> {
    </xsl:text>
            <xsl:for-each select="field">
                <xsl:if test="position() != 1">
                    <xsl:text>, </xsl:text>
                </xsl:if>
                <xsl:value-of select="@name"/>
            </xsl:for-each>
            <xsl:text>
}</xsl:text>
        </file>
    </xsl:template>

    <xsl:template name="codegen.service">
        <file name="{concat(@name,'.java')}">
            <xsl:call-template name="codegen.description">
                <xsl:with-param name="file-name" select="concat(@name,'.java')"/>
            </xsl:call-template>
            <xsl:call-template name="codegen.code"/>
        </file>
    </xsl:template>

    <xsl:template name="codegen.project.intellij_idea">
        <xsl:param name="app_files_path"/>

        <folder name="assets"/>
        <folder name="bin"/>
        <folder name="gen"/>
        <folder name="libs">
            <file path="../javaclient/weborbclient.jar"/>
        </folder>

        <file path="{concat($app_files_path, 'AndroidManifest.xml')}"/>
        <file path="{concat($app_files_path, 'build.xml')}"/>
        <file path="{concat($app_files_path, 'build.properties')}"/>
        <file path="{concat($app_files_path, 'default.properties')}"/>
        <file path="{concat($app_files_path, 'InvokerDemoApp.iml')}"/>
        <file path="{concat($app_files_path, 'local.properties')}"/>
        <file path="{concat($app_files_path, 'proguard.cfg')}"/>
        <folder name=".idea">
            <folder name="copyright">
                <file path="{concat($app_files_path, '.idea/copyright/profiles_settings.xml')}"/>
            </folder>
            <folder name="libraries">
                <file path="{concat($app_files_path, '.idea/libraries/weborbclient_jar.xml')}" hideContent="true"/>
            </folder>
            <file path="{concat($app_files_path, '.idea/.name')}"/>
            <file path="{concat($app_files_path, '.idea/ant.xml')}"/>
            <file path="{concat($app_files_path, '.idea/compiler.xml')}"/>
            <file path="{concat($app_files_path, '.idea/encodings.xml')}"/>
            <file path="{concat($app_files_path, '.idea/misc.xml')}"/>
            <file path="{concat($app_files_path, '.idea/modules.xml')}"/>
            <file path="{concat($app_files_path, '.idea/uiDesigner.xml')}"/>
            <file path="{concat($app_files_path, '.idea/vcs.xml')}"/>
            <file path="{concat($app_files_path, '.idea/workspace.xml')}"/>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.project.eclipse">
        <xsl:param name="app_files_path"/>

        <folder name="assets"/>
        <folder name="bin"/>
		<folder name="gen"/>
        <folder name="libs">
            <file path="../javaclient/weborbclient.jar"/>
        </folder>
        <file path="{concat($app_files_path, '.classpath')}"/>
        <file path="{concat($app_files_path, '.project')}"/>
        <file path="{concat($app_files_path, 'AndroidManifest.xml')}"/>
        <file path="{concat($app_files_path, 'default.properties')}"/>
        <file path="{concat($app_files_path, 'proguard.cfg')}"/>
        <folder name=".settings">
            <file path="{concat($app_files_path, '.settings/org.eclipse.jdt.core.prefs')}"/>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.project.res">
        <xsl:param name="res_files_path"/>

        <!-- processing 'res' files -->
        <folder name="res">
            <folder name="drawable-hdpi">
                <file path="{concat($res_files_path, 'drawable-hdpi/icon.png')}"/>
            </folder>
            <folder name="drawable-ldpi">
                <file path="{concat($res_files_path, 'drawable-ldpi/icon.png')}"/>
            </folder>
            <folder name="drawable-mdpi">
                <file path="{concat($res_files_path, 'drawable-mdpi/icon.png')}"/>
            </folder>
            <folder name="layout">
                <file path="{concat($res_files_path, 'layout/boolean_view.xml')}"/>
                <file path="{concat($res_files_path, 'layout/dialog_layout.xml')}"/>
                <file path="{concat($res_files_path, 'layout/expandable_list_view.xml')}"/>
                <file path="{concat($res_files_path, 'layout/methods_list_layout.xml')}"/>
                <file path="{concat($res_files_path, 'layout/map_entry_view.xml')}"/>
                <file path="{concat($res_files_path, 'layout/method_layout.xml')}"/>
                <file path="{concat($res_files_path, 'layout/methods_list_item_view.xml')}"/>
                <file path="{concat($res_files_path, 'layout/property_layout.xml')}"/>
                <file path="{concat($res_files_path, 'layout/result_layout.xml')}"/>
                <file path="{concat($res_files_path, 'layout/simple_type_view.xml')}"/>
            </folder>
            <folder name="values">
                <file path="{concat($res_files_path, 'values/strings.xml')}"/>
            </folder>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.project.src">
        <xsl:param name="src_files_path"/>
        <folder name="com">
            <folder name="example">
                <file path="{concat($src_files_path, 'com/example/MethodListView.java')}"/>
                <file path="{concat($src_files_path, 'com/example/MethodView.java')}"/>
                <file path="{concat($src_files_path, 'com/example/PropertyView.java')}"/>
                <file path="{concat($src_files_path, 'com/example/ResultView.java')}"/>
            </folder>
        </folder>
        <folder name="controllers">
            <file path="{concat($src_files_path, 'controllers/AbstractController.java')}"/>
            <file path="{concat($src_files_path, 'controllers/MethodController.java')}"/>
            <file path="{concat($src_files_path, 'controllers/MethodListController.java')}"/>
            <file path="{concat($src_files_path, 'controllers/PropertyController.java')}"/>
            <file path="{concat($src_files_path, 'controllers/ResultController.java')}"/>
        </folder>
        <folder name="models">
            <file name="AppModel.java">
                <xsl:text>package models;

import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Main model. Contains inspecting class, its methods, selected method, etc.
 *
 * @author Yuri Samsoniuk
 */
public class AppModel {
    /**
     * Class for class name
     */
    private Class clazz;
    /**
     * Inspecting class methods
     */
    private Method[] methods;
    /**
     * Selected method for invocation
     */
    public Method currentMethod = null;
    /**
     * Selected method invocation result type
     */
    public Type methodInvokationResultType = null;
    /**
     * Selected method invocation result
     */
    public Object methodInvokationResult = null;
    /**
     * Invocation result error message
     */
    public String errorMessage = null;
    /**
     * Invoking method arguments
     */
    public Object[] methodArguments = null;
    /**
     * WebORB Endpoint URL
     *
     * @see #loadProperties()
     * @see #saveProperties()
     */
    public String WebORBURL;
    /**
     * Invoking class name
     */
    public String invokingServiceClass = "</xsl:text><xsl:value-of select="//service/@fullname"/><xsl:text>";
    /**
     * Singleton model object
     */
    private static AppModel model = new AppModel();

    /**
     * Returns instance of the model
     *
     * @return model instance
     */
    public static AppModel getInstance() {
        return model;
    }

    /**
     * Constructor of the model. Called only once.
     */
    private AppModel() {
        try {
            clazz = Class.forName(invokingServiceClass);
            methods = clazz.getDeclaredMethods();
        } catch (ClassNotFoundException ignored) {
        }
        loadProperties();
    }

    /**
     * Loads WebORB Endpoint URL from system properties, if no such property, then sets to sample
     */
    private void loadProperties() {
        WebORBURL = System.getProperty("WebORBURL");
        if (WebORBURL == null) {
            WebORBURL = "</xsl:text>
                <xsl:variable name="url" select="//service/@url"/>
                <xsl:choose>
                    <xsl:when test="contains($url, 'localhost')">
                        <xsl:value-of select="substring-before($url, 'localhost')"/>
                        <xsl:text>10.0.2.2</xsl:text>
                        <xsl:value-of select="substring-after($url, 'localhost')"/>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of select="$url" /></xsl:otherwise>
                </xsl:choose>
                <xsl:text>";
        }
    }

    /**
     * Saves WebORB Endpoint URL to system properties
     */
    public void saveProperties() {
        System.setProperty("WebORBURL", WebORBURL);
    }

    /**
     * Returns description(method names and signatures) of the invoking class methods
     *
     * @param methodNameKey        key for method name
     * @param methodDescriptionKey key for description(signature)
     * @return description of the invoking class methods
     */
    public List&lt;Map&lt;String, String&gt;&gt; getMethodDescriptionList(String methodNameKey, String methodDescriptionKey) {
        List&lt;Map&lt;String, String>> methodDescriptionList = new ArrayList&lt;Map&lt;String, String&gt;&gt;();
        for (Method m : methods) {
            if (Modifier.isPublic(m.getModifiers())) {
                Map&lt;String, String> map = new HashMap&lt;String, String>();
                map.put(methodNameKey, m.getName());
                String description = m.getReturnType().getSimpleName() + " " + m.getName() + "(";
                Class[] parameterTypes = m.getParameterTypes();
                for (int j = 0; j &lt; parameterTypes.length - 1; j++) {
                    if (j == 0)
                        description += (parameterTypes[j].getSimpleName() + " arg" + j);
                    else
                        description += (", " + parameterTypes[j].getSimpleName() + " arg" + j);
                }
                description += ")";
                map.put(methodDescriptionKey, description);
                methodDescriptionList.add(map);
            }
        }

        return methodDescriptionList;
    }

    /**
     * Sets invoking method
     *
     * @param index index of method in method list
     */
    public void setCurrentMethod(int index) {
        currentMethod = methods[index];
    }
}
</xsl:text>
            </file>
            <file path="{concat($src_files_path, 'models/ArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/ArrayArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/BooleanArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/ComplexTypeArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/DateArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/EnumArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/GenericArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/MapArgInfo.java')}"/>
            <file path="{concat($src_files_path, 'models/PrimitiveArgInfo.java')}"/>
        </folder>
    </xsl:template>

    <xsl:template name="codegen.code">
        <xsl:text>
package </xsl:text><xsl:value-of select="@namespace"/><xsl:text>;

import weborb.client.Fault;
import weborb.client.IResponder;
import weborb.client.WeborbClient;
import weborb.exceptions.MessageException;

public class </xsl:text>
        <xsl:value-of select="@name"/><xsl:text> {
    private String weborbEndpointURL;
    
    public </xsl:text><xsl:value-of select="@name"/><xsl:text>(String weborbEndpointURL) {
        if (weborbEndpointURL == null) {
            this.weborbEndpointURL = "</xsl:text>
                <xsl:variable name="url" select="//service/@url"/>
                <xsl:choose>
                    <xsl:when test="contains($url, 'localhost')">
                        <xsl:value-of select="substring-before($url, 'localhost')"/>
                        <xsl:text>10.0.2.2</xsl:text>
                        <xsl:value-of select="substring-after($url, 'localhost')"/>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of select="$url" /></xsl:otherwise>
                </xsl:choose>
                <xsl:text>";
        } else {
            this.weborbEndpointURL = weborbEndpointURL;
        }
    }

    public </xsl:text><xsl:value-of select="@name"/><xsl:text>() {
        this(null);
    }
    </xsl:text>
        <xsl:for-each select="method">
            <xsl:call-template name="codegen.service.method"/>
        </xsl:for-each><xsl:text>
    private void handleInvoke(String methodName, Object[] args, IResponder responder) {
        WeborbClient client = new WeborbClient(weborbEndpointURL, "GenericDestination");
        try {
            client.invoke("</xsl:text><xsl:value-of select="@fullname"/><xsl:text>", methodName, args, responder);
        } catch (Exception e) {
            Fault fault = new Fault(e.getMessage(), e.toString());
            try {
                responder.errorHandler(fault);
            } catch (MessageException ignored) {
            }
        }
    }
}
</xsl:text>
    </xsl:template>

    <xsl:template name="codegen.service.method">
        <xsl:text>
    public void </xsl:text><xsl:value-of select="@name"/><xsl:text>(</xsl:text>
        <xsl:for-each select="arg">
            <xsl:if test="position() != 1">
                <xsl:text>, </xsl:text>
            </xsl:if>
            <xsl:value-of select="@javatype"/>
            <xsl:text> </xsl:text><xsl:value-of select="@name"/>
        </xsl:for-each>
        <xsl:if test="count(arg) != 0">
            <xsl:text>, </xsl:text>
        </xsl:if>
        <xsl:text>IResponder responder) {
        Object[] args = new Object[]{</xsl:text>
        <xsl:for-each select="arg">
            <xsl:if test="position() != 1">
                <xsl:text>, </xsl:text>
            </xsl:if>
            <xsl:value-of select="@name"/>
        </xsl:for-each><xsl:text>};
        handleInvoke("</xsl:text><xsl:value-of select="@name"/><xsl:text>", args, responder);
    }
    </xsl:text>
    </xsl:template>
</xsl:stylesheet>