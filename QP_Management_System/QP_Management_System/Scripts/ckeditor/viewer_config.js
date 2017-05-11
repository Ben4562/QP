/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    //config.uiColor = '#AADC6E';
    config.skin = "office2013";

    config.toolbar = 'Fullx';

    config.toolbar_Fullx =
   [
      ['Save','Copy', 'Print', 'SpellChecker', 'Find', '-', 'SelectAll', 'Maximize'],
      //['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'],
      //['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
      //['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
      //['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
      //['Link', 'Unlink', 'Anchor'],
      //['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar'],
      //['Styles', 'Format', 'Font', 'FontSize'],
      //['TextColor', 'BGColor'], 
   ];


    config.width = "850px"
    config.height = "400px"
    config.readOnly = true; CKEDITOR.editor.setReadOnly 
};
